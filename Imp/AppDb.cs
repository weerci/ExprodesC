using Calc.Data;
using Calc.Deprecated;
using Calc.Exceptions;
using Calc.Imp;
using Calc.Models;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.ViewModels.Settings;
using ExprodesC.Views.Controls;
using ExprodesC.Views.Synonym;
using ExprodesC.Wrappers;
using Func;
using Func.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using Tmds.DBus.Protocol;

namespace ExprodesC.Imp;

public class AppDb : CalcDb, IAppDb
{
    private ISettingsProvider<AppSettingsData> _settings;
    public AppDb(ISettingsProvider<AppSettingsData> settings) : base(settings)
    {
        _settings = settings;
    }

    /// <inheritdoc/>
    public IEnumerable<LocusAllele> GetAllPopLocuses()
    {
        using SQLiteCommand cmd = _settings.Value.Connect.CreateCommand();
        cmd.CommandText = "select name, ord, is_calc, mutant, allele_name, freq, min_freq, pop_id, id, allele_id, sname, is_synonym from LA_SYN; ";

        using SQLiteDataReader rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            yield return new LocusAllele
            {
                LName = rd.GetString(0),
                LOrd = rd.AsInt32(1),
                LCalc = rd.AsBoolean(2),
                MutFreq = rd.AsDouble(3),
                AName = rd.AsString(4),
                AFreq = rd.AsDouble(5),
                AMinFreq = rd.AsDouble(6),
                PopId = rd.AsInt32(7),
                LocusId = rd.AsInt32(8),
                AlleleId = rd.AsInt32(9),
                SName = rd.AsString(10),
                IsSynonym = rd.AsInt32(11),
            };
        }
    }

    /// <inheritdoc/>
    public int InsertPopulation(Population population, string baseName = "")
    {
        if (population == null)
            return 0;

        using (SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction())
        {
            string sql = @"INSERT INTO population(name, obj1) values (@name, @obj1); select last_insert_rowid(); ";
            using SQLiteCommand cmd = new SQLiteCommand(sql, _settings.Value.Connect, ft);

            cmd.Parameters.Add("@name", DbType.String).Value = population.Name;
            cmd.Parameters.Add("@obj1", DbType.String).Value = population.BaseOn;
            population.Id = Convert.ToInt32(cmd.ExecuteScalar());

            if (!String.IsNullOrEmpty(baseName))
            {
                cmd.Parameters.Clear();
                cmd.CommandText = "INSERT INTO pop_locus (pop_id, locus_id, min_freq, is_calc, mutant) select :pop_id, locus_id, min_freq, is_calc, mutant from pop_locus where pop_id = (select id from population where name = :base_name); ";
                cmd.Parameters.Add(":pop_id", DbType.Int32).Value = population.Id;
                cmd.Parameters.Add(":base_name", DbType.String).Value = baseName;
                cmd.ExecuteNonQuery();


                cmd.CommandText = "INSERT INTO pop_allele (pop_id, allele_id, freq, unknown) SELECT :pop_id, allele_id, freq, unknown FROM pop_allele where pop_id = (select id from population where name = :base_name); ";
                cmd.ExecuteNonQuery();
            }
            ft.Commit();
        }
        return population.Id;
    }

    /// <inheritdoc/>
    public int DeletePopulations(IEnumerable<Population> populations)
    {
        if (populations == null || !populations.Any())
            return 0;

        int result = 0;
        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        using SQLiteCommand cmd = new SQLiteCommand("delete from population where id = @id", _settings.Value.Connect, ft);
        cmd.Parameters.Add("@id", DbType.Int32);
        foreach (var item in populations)
        {
            cmd.Parameters[0].Value = item.Id;
            result += cmd.ExecuteNonQuery();
        }
        ft.Commit();
        return result;
    }

    /// <inheritdoc/>
    public void MovePopulation(Population from, Population to)
    {
        using SQLiteCommand cmd = _settings.Value.Connect.CreateCommand();
        cmd.CommandText = "update population set ord = @ord where id = @id_from; ";
        cmd.Parameters.Add("@ord", DbType.Int32).Value = to.Ord;
        cmd.Parameters.Add("@id_from", DbType.Int32).Value = from.Id;

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public void UpdatePopulation(Population population)
    {
        if (population == null)
            return;

        using SQLiteCommand cmd = _settings.Value.Connect.CreateCommand();
        cmd.CommandText = "update population set name = @name where id = @id";
        cmd.Parameters.Add("@id", DbType.UInt32).Value = population.Id;
        cmd.Parameters.Add("@name", DbType.String).Value = population.Name;
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public (int, int) InsertLocus(LocusWR lwr)
    {
        if (lwr == null)
            return (0, 0);

        const string findSql = "SELECT id, ord FROM locus WHERE name = @name;";

        using SQLiteTransaction transaction = _settings.Value.Connect.BeginTransaction();
        using SQLiteCommand? cmd = new SQLiteCommand(findSql, _settings.Value.Connect, transaction);

        cmd.Parameters.Add("@name", DbType.String).Value = lwr.Name;
        using (var reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                // Локус существует
                lwr.Id = reader.AsInt32(0);
                lwr.Ord = reader.AsInt32(1);
            }
            else
            {
                // Локус не существует - создаем новый
                const string insertSql = @"INSERT INTO LOCUS(name) 
                                         VALUES (@name); 
                                         SELECT last_insert_rowid();";

                using (var insertCmd = new SQLiteCommand(insertSql, _settings.Value.Connect, transaction))
                {
                    insertCmd.Parameters.Add("@name", DbType.String).Value = lwr.Name;
                    lwr.Id = Convert.ToInt32(insertCmd.ExecuteScalar());

                    // Получаем ord (предполагая, что это автоинкрементное поле)
                    lwr.Ord = lwr.Id;
                }
            }
        }

        cmd.Parameters.Clear();
        cmd.CommandText = "INSERT INTO pop_locus (pop_id, locus_id, min_freq, is_calc, mutant)  VALUES (:pop_id, :locus_id, :min_freq, :is_calc, :mutant); ";
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = lwr.PopId;
        cmd.Parameters.Add(":locus_id", DbType.Int32).Value = lwr.Id;
        cmd.Parameters.Add(":min_freq", DbType.Double).Value = lwr.MinFreq;
        cmd.Parameters.Add(":is_calc", DbType.Boolean).Value = lwr.IsCalc;
        cmd.Parameters.Add(":mutant", DbType.Double).Value = lwr.MutFreq;

        cmd.ExecuteNonQuery();
        transaction.Commit();

        return (lwr.Id, lwr.Ord);
    }

    /// <inheritdoc/>
    public void UpdateLocus(LocusWR lwr)
    {
        if (lwr == null)
            return;

        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();

        string sql = "update locus set name = @name where id = @id; ";
        using SQLiteCommand cmd = new SQLiteCommand(sql, _settings.Value.Connect, ft);

        cmd.Parameters.Add("@id", DbType.UInt32).Value = lwr.Id;
        cmd.Parameters.Add("@name", DbType.String).Value = lwr.Name;
        cmd.ExecuteNonQuery();

        cmd.Parameters.Clear();
        cmd.CommandText = "update pop_locus set is_calc = :is_calc, min_freq = :min_freq, mutant = :mutant where pop_id = :pop_id and locus_id = :locus_id; ";
        cmd.Parameters.Add(":is_calc", DbType.Int32).Value = Convert.ToInt32(lwr.IsCalc);
        cmd.Parameters.Add(":min_freq", DbType.Double).Value = lwr.MinFreq;
        cmd.Parameters.Add(":mutant", DbType.Double).Value = lwr.MutFreq;
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = lwr.PopId;
        cmd.Parameters.Add(":locus_id", DbType.Int32).Value = lwr.Id;
        cmd.ExecuteNonQuery();
        ft.Commit();
    }

    /// <inheritdoc/>
    public int DeleteLocus(IEnumerable<LocusWR> lwrs)
    {
        if (lwrs == null || !lwrs.Any())
            return 0;

        int result = 0;
        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        // Из таблицы Locus, элемент удаляется триггером ad_delete_locus
        using SQLiteCommand cmd = new SQLiteCommand("delete from pop_locus where locus_id = :id and pop_id = :pop_id; ", _settings.Value.Connect, ft);
        cmd.Parameters.Add(":id", DbType.Int32);
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = lwrs.ElementAt(0).PopId;

        foreach (var item in lwrs)
        {
            cmd.Parameters[0].Value = item.Id;
            result += cmd.ExecuteNonQuery();
        }
        ft.Commit();
        return result;

    }

    /// <inheritdoc/>
    public void MoveLocus(LocusWR from, LocusWR to)
    {
        using SQLiteCommand cmd = _settings.Value.Connect.CreateCommand();
        cmd.CommandText = "update locus set ord = @ord where id = @id_from; ";
        cmd.Parameters.Add("@ord", DbType.Int32).Value = to.Ord;
        cmd.Parameters.Add("@id_from", DbType.Int32).Value = from.Id;

        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>    
    public int InsertAllele(AlleleWR awr)
    {
        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        string sql = "insert into allele (name, locus_id) values (:name, :locus_id); select last_insert_rowid(); ";
        using SQLiteCommand cmd = new(sql, _settings.Value.Connect, ft);

        cmd.Parameters.Add(":name", DbType.String).Value = awr.Name;
        cmd.Parameters.Add(":locus_id", DbType.Int32).Value = awr.LocusId;
        awr.Id = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.Parameters.Clear();
        cmd.CommandText = "INSERT INTO pop_allele (pop_id, allele_id, freq, unknown) values (:pop_id, :allele_id, :freq, :unknown)";
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = awr.PopId;
        cmd.Parameters.Add(":allele_id", DbType.Int32).Value = awr.Id;
        cmd.Parameters.Add(":freq", DbType.Double).Value = awr.Freq;
        cmd.Parameters.Add(":unknown", DbType.Int32).Value = awr.Unknown;
        cmd.ExecuteNonQuery();

        ft.Commit();

        return awr.Id;
    }

    /// <inheritdoc/>
    public void UpdateAllele(AlleleWR awr)
    {
        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        using SQLiteCommand cmd = new SQLiteCommand("update allele set name = @name where id = @id; ", _settings.Value.Connect, ft);

        cmd.Parameters.Add("@id", DbType.Int32).Value = awr.Id;
        cmd.Parameters.Add("@name", DbType.String).Value = awr.Name;
        cmd.ExecuteNonQuery();

        cmd.Parameters.Clear();
        cmd.CommandText = "update pop_allele set freq = :freq, unknown = :unknown where pop_id = :pop_id and allele_id = :allele_id; ";
        cmd.Parameters.Add(":freq", DbType.Double).Value = awr.Freq;
        cmd.Parameters.Add(":unknown", DbType.Boolean).Value = false;
        cmd.Parameters.Add(":allele_id", DbType.Int32).Value = awr.Id;
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = awr.PopId;
        cmd.ExecuteNonQuery();
        ft.Commit();
    }

    /// <inheritdoc/>
    public int DeleteAllele(IEnumerable<AlleleWR> awrs)
    {
        if (awrs == null || !awrs.Any())
            return 0;

        int result = 0;

        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        using SQLiteCommand cmd = new("delete from pop_allele where allele_id = :id and pop_id = :pop_id; ", _settings.Value.Connect, ft);

        cmd.Parameters.Add(":id", DbType.Int32);
        cmd.Parameters.Add(":pop_id", DbType.Int32).Value = awrs.ElementAt(0).PopId;
        foreach (var item in awrs)
        {
            cmd.Parameters[0].Value = item.Id;
            result += cmd.ExecuteNonQuery();
        }
        ft.Commit();
        return result;
    }

    /// <inheritdoc/>
    public IEnumerable<SynonymWR> GetAllSynonyms()
    {
        using SQLiteCommand cmd = new("select s.id, s.name, l.id, l.name, l.ord from synonym s left join locus l on l.id = s.locus_id;", _settings.Value.Connect);

        SQLiteDataReader r = cmd.ExecuteReader();
        while (r.Read())
            yield return new SynonymWR(r.AsInt32(0), r.AsString(1),  r.AsInt32(2), r.AsString(3), r.AsInt32(4));
    }

    /// <inheritdoc/>
    public int InsertSynonym(SynonymWR swr)
    {
        using SQLiteCommand cmd = new("insert into synonym (name, locus_id) values (@name, @locus_id); select last_insert_rowid(); ", _settings.Value.Connect);

        cmd.Parameters.Add("@name", DbType.String).Value = swr.Name;
        cmd.Parameters.Add("@locus_id", DbType.Int32).Value = swr.LocusId;

        return cmd.ExecuteScalar().AsInt64();

    }

    /// <inheritdoc/>
    public void UpdateSynonym(SynonymWR swr)
    {
        using SQLiteCommand cmd = new("update synonym set name = @name, locus_id = @locus_id where id = @id; ", _settings.Value.Connect);

        cmd.Parameters.Add("@id", DbType.Int32).Value = swr.Id;
        cmd.Parameters.Add("@name", DbType.String).Value = swr.Name;
        cmd.Parameters.Add("locus_id", DbType.Int32).Value = swr.LocusId;
        cmd.ExecuteNonQuery();
    }

    /// <inheritdoc/>
    public int DeleteSynonym(IEnumerable<SynonymWR> swrs)
    {
        if (!swrs.Any())
            return 0;

        using SQLiteTransaction ft = _settings.Value.Connect.BeginTransaction();
        using SQLiteCommand cmd = new SQLiteCommand("delete from synonym where id = @id", _settings.Value.Connect, ft);

        int result = 0;

        cmd.Parameters.Add("@id", DbType.Int32);
        foreach (var item in swrs)
        {
            cmd.Parameters[0].Value = item.Id;
            result += cmd.ExecuteNonQuery();
        }
        ft.Commit();
        return result;
    }

}

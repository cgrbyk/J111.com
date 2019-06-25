using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using J111.Models;

namespace J111.Controllers
{
    public class DatabaseConnection
    {
        OracleConnection sqlConnection;

        public DatabaseConnection()
        {
            string constring= ConfigurationManager.ConnectionStrings["localString"].ConnectionString;
            sqlConnection = new OracleConnection(constring);
        }
        public DataTable GetDataTable(string quarry, string[] parametreler)
        {
            OracleCommand command = new OracleCommand(quarry, sqlConnection);
            if(parametreler!=null)
            for (int i = 0; i < parametreler.Length; i++)
                command.Parameters.Add(new OracleParameter("p"+i.ToString(),parametreler[i]));
            DataTable dt = new DataTable();
            OracleDataAdapter oda = new OracleDataAdapter(command);
            sqlConnection.Open();
            oda.Fill(dt);
            sqlConnection.Close();
            return dt;
        }
        public void ExecuteSqlQuarry(string quarry, string[] parametreler)
        {
            OracleCommand command = new OracleCommand(quarry, sqlConnection);
            if (parametreler != null)
                for (int i = 0; i < parametreler.Length; i++)
                    command.Parameters.Add(new OracleParameter("p" + i.ToString(), parametreler[i]));
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sqlConnection.Close();

        }
        public string UDTuserregister(kullaniciGiris kul)
        {
            string quarry = "KULLANICIPAC.EKLE";
            string msg="Kayıt başarılı";
            OracleCommand Okomut = new OracleCommand();
            try
            {             
                Okomut.Connection = sqlConnection;
                Okomut.CommandText = quarry;
                Okomut.CommandType = CommandType.StoredProcedure;

                Okomut.Parameters.Add("KULEMAIL",OracleDbType.NVarchar2, ParameterDirection.Input).Value=kul.email;     
                Okomut.Parameters.Add("KULADI", OracleDbType.NVarchar2, ParameterDirection.Input).Value = kul.ad;       
                Okomut.Parameters.Add("KULSOYADI", OracleDbType.NVarchar2, ParameterDirection.Input).Value = kul.soyad; 
                Okomut.Parameters.Add("KULSIFRE", OracleDbType.NVarchar2, ParameterDirection.Input).Value = kul.sifre;  
                Okomut.Parameters.Add("KULSIFRE2", OracleDbType.NVarchar2, ParameterDirection.Input).Value = kul.sifre2;

                Okomut.Parameters.Add("error_msg",OracleDbType.Clob,ParameterDirection.Output);
                sqlConnection.Open();
                Okomut.ExecuteNonQuery();
                object Message = Okomut.Parameters["error_msg"].Value;
                msg = ((OracleClob)Message).Value;
            }
            catch (OracleException e)
            {
               msg = e.Message.ToString();                       
            }
            finally {               
                sqlConnection.Close();           
            }
            return msg;
        }
        public string AdresEkle(adresData aD)
        {
            //string quarry = "ADRESPAC.ADRESEKLE";
            string msg = "Kayıt başarılı";
            OracleCommand Okomut =new OracleCommand();

            OracleParameter p_ad = new OracleParameter("adlar", OracleDbType.Varchar2);
            OracleParameter p_acik = new OracleParameter("aciklar", OracleDbType.Varchar2);
            try
            {
                // create command object and set attributes
                Okomut.Connection = sqlConnection;
                Okomut.CommandText = "ADRESPAC.ADRESEKLE";
                Okomut.CommandType = CommandType.StoredProcedure;

                Okomut.Parameters.Add("kul_mail", OracleDbType.NVarchar2, ParameterDirection.Input).Value = aD.KULLANICIEMAIL;
                Okomut.Parameters.Add("kul_sifre", OracleDbType.NVarchar2, ParameterDirection.Input).Value = aD.KULLANICISIFRE;
                
                // set the collection type for each parameter
                p_ad.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                p_acik.CollectionType = OracleCollectionType.PLSQLAssociativeArray;

                // set the parameter values
                p_ad.Value = aD.AdresAdıD;
                p_acik.Value = aD.AdresAcıD;

                // set the size for each array
                p_ad.Size = aD.AdresAdıD.Length;
                p_acik.Size = aD.AdresAcıD.Length;

                // add parameters to command object collection
                Okomut.Parameters.Add(p_ad);
                Okomut.Parameters.Add(p_acik);

                Okomut.Parameters.Add("msg", OracleDbType.Clob, ParameterDirection.Output);

                // execute the insert
                sqlConnection.Open();
                Okomut.ExecuteNonQuery();
                object Message = Okomut.Parameters["msg"].Value;
                msg = ((OracleClob)Message).Value;


            }
            catch (OracleException e)
            {
                msg = e.Message.ToString();                        
            }
            finally
            {
                // clean up 
                p_ad.Dispose();
                p_acik.Dispose();
                Okomut.Dispose();
                sqlConnection.Dispose();
                sqlConnection.Close();
            }
            return msg;
        }
        public string ResimEkle(string urunıd, byte[] resim)
        {
            string msg=string.Empty;
            string Quarry = "INSERT INTO URUNRESIM VALUES(:pid,:presim)";
            OracleCommand Okomut = new OracleCommand(Quarry,sqlConnection);
            try
            {
                OracleParameter p_urunid = new OracleParameter(":pid", OracleDbType.Int32);
                p_urunid.Value = urunıd;
                Okomut.Parameters.Add(p_urunid);
                OracleParameter p_resim = new OracleParameter(":presim", OracleDbType.Blob);
                p_resim.Value = resim;
                Okomut.Parameters.Add(p_resim);
                sqlConnection.Open();
                Okomut.ExecuteNonQuery();
                //object Message = Okomut.Parameters["msg"].Value;
                //msg = ((OracleClob)Message).Value;
            }
            catch (OracleException ex)
            {
                msg = ex.Message;
            }
            finally {
                sqlConnection.Close();
            }
            return msg;
        }
        public byte[] resimAl(string UrunID)
        {
            string msg = string.Empty;
            string Quarry = "SELECT BIMAGE FROM URUNRESIM WHERE URUNID=:puid";
            OracleCommand Okomut = new OracleCommand(Quarry,sqlConnection);
            byte[] resim=null;
            try
            {
                OracleParameter p_uid = new OracleParameter("puid",OracleDbType.Int32);
                p_uid.Value = UrunID;
                Okomut.Parameters.Add(p_uid);
                sqlConnection.Open();
                OracleDataReader dr = Okomut.ExecuteReader();
                dr.Read();
                resim = (byte[])(dr.GetOracleBlob(0)).Value;
            }
            catch (OracleException ex)
            {
                msg = ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
            return resim;
        }
    }
}
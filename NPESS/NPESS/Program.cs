using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


/*
 *File Name: NPPES.SLN
 *Purpose: Automation of Monthly NPPESS load. 
 * Authour: Eric Lane(AF32634) 
 * Initial Implementation: 03/12/2020
 */



namespace NPPES
{
    class Program
    {
        static void Main(string[] args)
        {

            //int currentMonth = DateTime.Now.Month;
            int currentMonth = 8;//For testing only. Delete when live. 
            string currentYear = DateTime.Now.Year.ToString();



            IDictionary<int, string> dict = new Dictionary<int, string>()

                        /*
 *Dictionary used to convert int(month) to string(month) 
 */
                                            {
                                                {1,"January"},
                                                {2, "February"},
                                                {3,"March"},
                                                {4,"April"},
                                                {5, "May"},
                                                {6,"June"},
                                                {7,"July"},
                                                {8, "August"},
                                                {9,"September"},
                                                { 10, "October"},
                                                { 11,"November"},
                                                { 12,"December"}


             };


            string rootDirectory = @"\\agpcorp\files\VA4\Longterm\PTS\PROD\NPIPSD\Monthly";  //direc
            string fileName = "NPPES_Data_Dissemination_" + dict[currentMonth] + "_" + currentYear + ".zip";

            string zipPath = rootDirectory + @"\" + fileName;
            string unzipSubfolder = "NPPES_Data_Dissemination_" + dict[currentMonth] + "_" + currentYear;
            string extractPath = @"\\agpcorp\files\VA4\Longterm\PTS\PROD\NPIPSD\Monthly\Unzip" + @"\" + unzipSubfolder;


            //System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath); //Unzips NPPES file. 




            getNppesFileName(extractPath, unzipSubfolder);


        }

        public static void getNppesFileName(string path, string subfolder)
        {


            /*
          *This function gets the name of the current NPPES file. 
          */

            DirectoryInfo d = new DirectoryInfo(path);//Assuming Test is your Folder
            FileInfo[] files = d.GetFiles("*.*"); //Getting all files in directory

            long size = 0;
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains("npidata") & !(file.Name.Contains("FileHeader")))
                {
                    size = file.Length;
                    string names = file.Name;  // 

                    CreatePropertyFile(names, path, size, subfolder);

                    // 
                }



            }
        }



        public static void CreatePropertyFile(string fileName, string path, long size, string subfolder)
        {

            // Fuction to create property files. 
            using (StreamWriter sw = File.CreateText(path + @"\" + fileName + ".properties"))
            {

                sw.WriteLine("RRM_SenderID=EDSCMS");
                sw.WriteLine("RRM_ReceiverID=EDIFECSMAOAPP");
                sw.Close();
            }



            copyFiles(fileName, path, size);


        }


        public static void copyFiles(string fileName, string path, long size)
        {

            string prodDropPath = @"\\va01pstodfs003.corp.agp.ads\apps\Edifects_prod\EM\Inbound\Provider\Generic\NPPES_Provider_CSV";   // Production hot folder. 
                                                                                                                                        // string prodDropPath = @"\\agpcorp\files\VA4\Longterm\PTS\PROD\NPIPSD\Monthly\Unzip";// test location 



            File.Copy(path + @"\" + fileName + ".properties", prodDropPath + @"\" + fileName + ".properties", true);
            Thread.Sleep(6000); //puase thread to copy over property file first. 
            File.Move(path + @"\" + fileName, prodDropPath + @"\" + fileName);
        }


    }
}


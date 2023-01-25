using System;
using System.IO;
using Azure.Storage.Blobs;
using ConsoleApp1.Helpers;
using System.Collections.Generic;
namespace ConsoleApp1
{
    internal class Program
    {
        static  void Main(string[] args)
        {
            var helpers = new Helpers.Helpers();
            Dictionary<string, string> blobFileNames = helpers.CreateBlobs(5);
            Console.WriteLine("Blobs were created.");
            helpers.UploadBlobs(helpers.GetfirstLocalStorageContainerName(), blobFileNames);
            Console.WriteLine("Blobs were Uploaded to the first storage account.");
            helpers.CopyFromFirstStorageAccountToSecondStorageAccount(helpers.GetfirstLocalStorageContainerName(), helpers.GetsecondLocalStorageContainerName(), blobFileNames);
            Console.WriteLine("Blobs were copied from first storage account to the second storage account.");
            helpers.DeleteLocalBlobs();
            Console.WriteLine("Blobs were deleted.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Azure.Storage.Blobs;

namespace ConsoleApp1.Helpers
{
    public class Helpers
    {
        #region Class Attributes
        protected string localPath = "C:\\Users\\managementadmin\\HomeAssignment\\ConsoleApp1\\blobs\\";
        protected string localStorageOneConnectionString = "DefaultEndpointsProtocol=https;AccountName=firstserver;AccountKey=aBiAkdTZYZS9IZ9kSC/OnVkXM0NBzR36Uf9J2WAXB8aV32H71c+k+Qg2n8FqH5MySE44icU0QAY5+AStQGKj8w==;EndpointSuffix=core.windows.net";
        protected string localStorageTwoConnectionString = "DefaultEndpointsProtocol=https;AccountName=secondserver;AccountKey=HKQ4T+GGnfXMRjXDncTsi7n3ol2WLSnUy02Sepg+ZKHsQvzeUBOhUN9fgYDeW5Lp9yxewgHtlVvi+AStaNSL9w==;EndpointSuffix=core.windows.net";
        protected string firstLocalStorageContainerName = "tamirserver1container";
        protected string secondLocalStorageContainerName = "secondservercontainer";
        #endregion Class Attributes

        #region Getters
        public string GetLocalStorageOneConnectionString()
        {
            return this.localStorageOneConnectionString;
        }
        public string GetLocalStorageTwoConnectionString()
        {
            return this.localStorageTwoConnectionString;
        }
        public string GetfirstLocalStorageContainerName()
        {
            return this.firstLocalStorageContainerName;
        }
        public string GetsecondLocalStorageContainerName()
        {
            return this.secondLocalStorageContainerName;
        }
        #endregion Getters


        #region Minor Methods
        private BlobContainerClient setContainer(string connectionString, string containerName)
        {
            return new BlobContainerClient(connectionString, containerName);
        }
        /// <summary>
        /// Copies one blob from the first storage account to the second storage account.
        /// </summary>
        /// <param name="sourceContainer"></param>
        /// <param name="destinationContainer"></param>
        /// <param name="blobName"></param>
        private void CopyBlob(string sourceContainer, string destinationContainer, string blobName)
        {
            var sourceStorageAccountContainer = setContainer(this.localStorageOneConnectionString, sourceContainer);
            var destinationStorageAccountContainer = setContainer(this.localStorageTwoConnectionString, destinationContainer);
            var sourceBlobClient = sourceStorageAccountContainer.GetBlobClient(blobName);
            var destinationBlobClient = destinationStorageAccountContainer.GetBlobClient(blobName);
            destinationBlobClient.StartCopyFromUri(sourceBlobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.Now.AddHours(1)));
        }

        /// <summary>
        ///  Uploading a blob into storageServer1 - in this task we are only uploading to the first storage account - hence im not getting the connection string
        ///  through the params of the method. if we did not know to which storage account we were uploading - i would given another param with the connection string.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="fileName"></param>
        private void UploadBlob(string containerName,string blobName,string fileName)
        {
            var container = setContainer(this.localStorageOneConnectionString, containerName);  
            var blob = container.GetBlobClient(blobName);
            var stream = File.OpenRead(this.localPath + fileName);
            blob.Upload(stream);
            stream.Close();
        }

        #endregion Minor Methods

        #region Major Methods
        public void UploadBlobs(string containerName, Dictionary<string, string> blobFileNames)
        {
            var container = new BlobContainerClient(this.localStorageOneConnectionString, containerName);
            foreach (var item in blobFileNames)
            {
                UploadBlob(containerName, item.Key, item.Value);
            }
        }

        /// <summary>
        /// Creats {amount} blobs - for this task - all ill do is create some .txt files to upload to the first storage account.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Dictionary<string,string> CreateBlobs(int amount)
        {
            Random rd = new Random();
            var fileNamePrefix = "TamirTextFile";
            Dictionary<string,string> blobFileNames = new Dictionary<string,string>(); 
            for(int i = 1; i <= amount; i++)
            {
                var fileName = fileNamePrefix + rd.Next(1, 1000000).ToString() + ".txt";
                blobFileNames.Add(fileName, fileName);
                using (FileStream fs = File.Create(localPath + fileName))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes("just some title");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("Tamir Gonen");
                    fs.Write(author, 0, author.Length);
                }
            }
            return blobFileNames;
        }



        /// <summary>
        /// Runs CopyBlob on all the blobs in the dictionary.
        /// 
        /// Note that usually the key and the value in the dictionary are the same but in general this is not always the case, but the key represents the 
        /// blob name while the value represents the filename on the local machine/server.
        /// </summary>
        /// <param name="firstStorageContainerName"></param>
        /// <param name="secondStorageContainerName"></param>
        /// <param name="blobFileNames"></param>
        public void CopyFromFirstStorageAccountToSecondStorageAccount(string firstStorageContainerName, string secondStorageContainerName, Dictionary<string, string> blobFileNames)
        {
            foreach(var item in blobFileNames)
            {
                CopyBlob(firstStorageContainerName, secondStorageContainerName, item.Key);
            }
        }

        /// <summary>
        /// We don't need to store those blobs locally since we upload it to one storage account and copy them to the second storage account as backup.
        /// </summary>
        public void DeleteLocalBlobs()
        {
            var files = Directory.GetFiles(localPath);
            foreach(var file in files)
            {
                File.Delete(file);
            }
        }
        #endregion Major Methods
    }
}

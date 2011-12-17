using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace MinusEngine
{

    #region TODO

    //TODO: Complete GetFoldersResult Class
    //TODO: Complete GetFilesResult Class

    #endregion

    #region Handler Delegates

    // ReSharper disable InconsistentNaming
    public delegate void oAuthCompleteHandler(MinusEngine sender, oAuthResult result);
    public delegate void oAuthFailedHandler(MinusEngine sender, Exception e);
    // ReSharper restore InconsistentNaming

    public delegate void UploadItemCompleteHandler(MinusEngine sender, UploadItemResult result);
    public delegate void UploadItemFailedHandler(MinusEngine sender, Exception e);

    public delegate void CreateFolderCompleteHandler(MinusEngine sender, CreateFolderResult result);
    public delegate void CreateFolderFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFoldersCompleteHandler(MinusEngine sender, GetFoldersResult result);
    public delegate void GetFoldersFailedHandler(MinusEngine sender, Exception e);

    public delegate void FolderCompleteHandler(MinusEngine sender, FolderResult result);
    public delegate void FolderFailedHandler(MinusEngine sender, Exception e);

    public delegate void FileCompleteHandler(MinusEngine sender, FileResult result);
    public delegate void FileFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFilesCompleteHandler(MinusEngine sender, GetFilesResult result);
    public delegate void GetFilesFailedHandler(MinusEngine sender, Exception e);

    #endregion

    public class MinusEngine
    {

        // ReSharper disable InconsistentNaming
        public static readonly String BASE_URL = "https://minus.com/";
        public static readonly Uri AUTH_URL = new Uri(BASE_URL + "oauth/token");
        public static readonly Uri FOLDERS_URL = new Uri(BASE_URL + "api/v2/folders/");
        public static readonly Uri USERS_URL = new Uri(BASE_URL + "api/v2/users/");
        public static readonly Uri FILES_URL = new Uri(BASE_URL + "api/v2/files/");
        // ReSharper restore InconsistentNaming

        #region Handler Events

        // ReSharper disable InconsistentNaming
        public event oAuthCompleteHandler oAuthComplete;
        public event oAuthFailedHandler oAuthFailed;
        // ReSharper restore InconsistentNaming

        public event UploadItemCompleteHandler UploadItemComplete;
        public event UploadItemFailedHandler UploadItemFailed;

        public event CreateFolderCompleteHandler CreateFolderComplete;
        public event CreateFolderFailedHandler CreateFolderFailed;

        public event GetFoldersCompleteHandler GetFoldersComplete;
        public event GetFoldersFailedHandler GetFoldersFailed;

        public event FolderCompleteHandler FolderComplete;
        public event FolderFailedHandler FolderFailed;

        public event FileCompleteHandler FileComplete;
        public event FileFailedHandler FileFailed;

        public event GetFilesCompleteHandler GetFilesComplete;
        public event GetFilesFailedHandler GetFilesFailed;

        #endregion

        #region Authorization

        // ReSharper disable InconsistentNaming
        public void oAuth(String username, String password, String clientId, String secret, String access)
        // ReSharper restore InconsistentNaming
        {
            StringBuilder data = new StringBuilder();
            data.Append("grant_type=password&client_id=" + clientId + "&client_secret=" + secret +
                               "&scope=" + access + "&username=" + username + "&password=" + password);

            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadString(AUTH_URL, "POST", data.ToString());
                    }
                    catch (Exception e)
                    {
                        TriggeroAuthFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggeroAuthFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("oAuth operation failed: " + e.Error.Message);
                    TriggeroAuthFailed(e.Error);
                    return;
                }

                oAuthResult result = JsonConvert.DeserializeObject<oAuthResult>(e.Result);
                Debug.WriteLine("oAuth operation successful: " + result);
                TriggeroAuthComplete(result);
            };
        }

        #endregion

        #region Folders

        #region Gets

        public void GetFolders(String username, String accessToken)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFolders = new Uri(USERS_URL + username + "/folders?bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getFolders);
                    }
                    catch (WebException e)
                    {
                        TriggerGetFoldersFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFoldersFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Folders operation failed: " + e.Error.Message);
                    TriggerCreateFolderFailed(e.Error);
                    return;
                }
                    
                GetFoldersResult result = JsonConvert.DeserializeObject<GetFoldersResult>(e.Result);
                Debug.WriteLine("Get Folders operation successful: " + result);
                TriggerGetFoldersComplete(result);
            };
        }

        public void GetFolder(String folderID, String accessToken)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFolder = new Uri(FOLDERS_URL + folderID + "?bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                 {
                    try
                    {
                        client.DownloadStringAsync(getFolder);
                    }
                    catch (WebException e)
                    {
                        TriggerFolderFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerFolderFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Folder operation failed: " + e.Error.Message);
                    TriggerCreateFolderFailed(e.Error);
                    return;
                }

                FolderResult result = JsonConvert.DeserializeObject<FolderResult>(e.Result);
                Debug.WriteLine("Get Folder operation successful: " + result);
                TriggerFolderComplete(result);
           };
        }

        #endregion

        #region Modify/Delete

        public void ModifyFolder(String folderID, String name, String[] itemOrdering, String accessToken)
        {
            Uri folder = new Uri(FOLDERS_URL + folderID + "?bearer_token=" + accessToken);

            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();
            data.Append("name=" + name + "&item_ordering=" + itemOrdering);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadStringAsync(folder, "PUT", data.ToString());
                    }
                    catch (Exception e)
                    {
                        TriggerFolderFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerFolderFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Modify Folder operation failed: " + e.Error.Message);
                    TriggerFolderFailed(e.Error);
                    return;
                }

                FolderResult result = JsonConvert.DeserializeObject<FolderResult>(e.Result);
                Debug.WriteLine("Modify Folder operation successful: " + result);
                TriggerFolderComplete(result);
           };
        }

        public void DeleteFolder(String folderID, String accessToken)
        {
            Uri folder = new Uri(FOLDERS_URL + folderID + "?bearer_token=" + accessToken);

            CookieAwareWebClient client = new CookieAwareWebClient();

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.deleteRequest(folder);
                        Debug.WriteLine("Delete Folder operation successful");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Delete Folder operation failed: " + e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
            }

        }

        #endregion

        #region Create

        public void CreateFolder(String username, String accessToken, String name, Boolean isPublic)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();

            Uri createFolder = new Uri(USERS_URL + username + "/folders?bearer_token=" + accessToken);

            data.Append("name=" + name + "&is_public=" + isPublic);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        Debug.WriteLine("Before Create Folder");
                        client.UploadStringAsync(createFolder, "POST", data.ToString());
                        Debug.WriteLine("After Create Folder");
                    }
                    catch (WebException e)
                    {
                        TriggerCreateFolderFailed(e);
                    }
               }
               );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerCreateFolderFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Create Folder operation failed: " + e.Error.Message);
                    TriggerCreateFolderFailed(e.Error);
                    return;
                }

                CreateFolderResult result = JsonConvert.DeserializeObject<CreateFolderResult>(e.Result);
                Debug.WriteLine("Create Folder operation successful: " + result);
                TriggerCreateFolderComplete(result);
            };
        }

        #endregion

        #endregion

        #region Files

        #region Gets

        public void GetFile(String fileID, String accessToken)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFolder = new Uri(FILES_URL + fileID + "?bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        Debug.WriteLine("Before Get File");
                        client.DownloadStringAsync(getFolder);
                        Debug.WriteLine("After Get File");
                    }
                    catch (WebException e)
                    {
                        TriggerFileFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerFileFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get File operation failed: " + e.Error.Message);
                    TriggerFileFailed(e.Error);
                    return;
                }

                FileResult result = JsonConvert.DeserializeObject<FileResult>(e.Result);
                Debug.WriteLine("Get File operation successful: " + result);
                TriggerFileComplete(result);
            };
        }

        public void GetFiles(String folderID, String accessToken)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFiles = new Uri(FOLDERS_URL + folderID + "/files?bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getFiles);

                    }
                    catch (WebException e)
                    {
                        TriggerGetFilesFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFilesFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Files operation failed: " + e.Error.Message);
                    TriggerGetFilesFailed(e.Error);
                    return;
                }

                GetFilesResult result = JsonConvert.DeserializeObject<GetFilesResult>(e.Result);
                Debug.WriteLine("Get Folders operation successful: " + result);
                TriggerGetFilesComplete(result);
            };
        }

        #endregion

        #region Modify/Delete

        public void ModifyFile(String fileID, String caption, String accessToken)
        {
            Uri file = new Uri(FILES_URL + fileID + "?bearer_token=" + accessToken);

            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();
            data.Append("caption=" + caption);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadStringAsync(file, "PUT", data.ToString());
                    }
                    catch (Exception e)
                    {
                        TriggerFileFailed(e);
                    }
                }
                    );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerFileFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Modify File operation failed: " + e.Error.Message);
                    TriggerFileFailed(e.Error);
                    return;
                }

                FileResult result = JsonConvert.DeserializeObject<FileResult>(e.Result);
                Debug.WriteLine("Modify File operation successful: " + result);
                TriggerFileComplete(result);
            };
        }

        public void DeleteFile(String fileID, String accessToken)
        {
            Uri folder = new Uri(FILES_URL + fileID + "?bearer_token=" + accessToken);

            CookieAwareWebClient client = new CookieAwareWebClient();

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.deleteRequest(folder);
                        Debug.WriteLine("Delete File operation successful");

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Delete File operation failed: " + e);
                    }
                }
                    );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
            }

        }

        #endregion

        #region Uploads

        public void UploadItem(String accessToken, String folderId, String filePath)
        {
            UploadItem(accessToken, folderId, null, filePath);
        }

        public void UploadItem(String accessToken, String folderId, String caption, String filePath)
        {
            // Not worth checking for file existence or other stuff, as either Path.GetFileName or the upload
            // will check & fail
            Stream data = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            UploadItem(accessToken, folderId, filePath, caption, data);
        }

        public void UploadItem(String accessToken, String folderId, String caption, String filePath, Stream data)
        {
            Uri upload = new Uri(FOLDERS_URL + folderId + "/files?bearer_token=" + accessToken);
            const string boundaryString = "------WebKitFormBoundaryAYAOHDWfizxZB8OE";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(upload);
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundaryString;
                request.KeepAlive = true;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        MemoryStream postDataStream = new MemoryStream();
                        StreamWriter postDataWriter = new StreamWriter(postDataStream);
                        string fileName = Path.GetFileName(filePath);

                        //File
                        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                        postDataWriter.Write("Content-Disposition: form-data;"
                                             + "name=\"{0}\"; "
                                             + "filename=\"{1}\""
                                             + "\r\nContent-Type: {2}\r\n\r\n",
                                             "file",
                                             fileName,
                                             Path.GetExtension(filePath));

                        byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)data.Length))];
                        postDataWriter.Flush();
                        PushData(data, postDataStream, buffer);
                        data.Close();

                        //Filename
                        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                        postDataWriter.Write("Content-Disposition: form-data;"
                                             + " name=\"{0}\"\r\n\r\n{1}",
                                             "filename",
                                             fileName);

                        //Caption
                        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                        postDataWriter.Write("Content-Disposition: form-data;"
                                             + " name=\"{0}\"\r\n\r\n{1}",
                                             "caption",
                                             caption);

                        postDataWriter.Flush();



                        postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");
                        postDataWriter.Flush();

                        request.ContentLength = postDataStream.Length;

                        Stream strmOut = request.GetRequestStream();

                        postDataStream.WriteTo(strmOut);



                        WebResponse response = request.GetResponse();
                        Stream stream = response.GetResponseStream();

                        if (stream != null)
                        {
                            StreamReader reader = new StreamReader(stream);
                            string responseString = reader.ReadToEnd();

                            UploadItemResult resultItems = JsonConvert.DeserializeObject<UploadItemResult>(responseString);
                            Debug.WriteLine("UploadItem operation successful: " + resultItems);
                            TriggerUploadItemComplete(resultItems);
                        }

                        postDataStream.Close();
                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine("Upload Failed" + e.Message);
                        TriggerUploadItemFailed(e);
                    }


                });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerUploadItemFailed(e);
            }
        }

        #endregion

        #endregion

        #region Helpers

        private static void PushData(Stream input, Stream output, byte[] imageBuffer)
        {
            byte[] buffer = imageBuffer;
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        #endregion

        #region Triggers

        private void TriggeroAuthComplete(oAuthResult result)
        {
            if (oAuthComplete != null)
            {
                oAuthComplete.Invoke(this, result);
            }
        }

        private void TriggeroAuthFailed(Exception e)
        {
            if (oAuthFailed != null)
            {
                oAuthFailed.Invoke(this, e);
            }
        }

        private void TriggerUploadItemComplete(UploadItemResult result)
        {
            if (UploadItemComplete != null)
            {
                UploadItemComplete.Invoke(this, result);
            }
        }

        private void TriggerUploadItemFailed(Exception e)
        {
            if (UploadItemFailed != null)
            {
                UploadItemFailed.Invoke(this, e);
            }
        }

        private void TriggerCreateFolderComplete(CreateFolderResult result)
        {
            if (CreateFolderComplete != null)
            {
                CreateFolderComplete.Invoke(this, result);
            }
        }

        private void TriggerCreateFolderFailed(Exception e)
        {
            if (CreateFolderFailed != null)
            {
                CreateFolderFailed.Invoke(this, e);
            }
        }

        private void TriggerGetFoldersComplete(GetFoldersResult result)
        {
            if (GetFoldersComplete != null)
            {
                GetFoldersComplete.Invoke(this, result);
            }
        }

        private void TriggerGetFoldersFailed(Exception e)
        {
            if (GetFoldersFailed != null)
            {
                GetFoldersFailed.Invoke(this, e);
            }
        }

        private void TriggerFolderComplete(FolderResult result)
        {
            if (FolderComplete != null)
            {
                FolderComplete.Invoke(this, result);
            }
        }

        private void TriggerFolderFailed(Exception e)
        {
            if (FolderFailed != null)
            {
                FolderFailed.Invoke(this, e);
            }
        }

        private void TriggerFileComplete(FileResult result)
        {
            if (FileComplete != null)
            {
                FileComplete.Invoke(this, result);
            }
        }

        private void TriggerFileFailed(Exception e)
        {
            if (FileFailed != null)
            {
                FileFailed.Invoke(this, e);
            }
        }

        private void TriggerGetFilesComplete(GetFilesResult result)
        {
            if (GetFilesComplete != null)
            {
                GetFilesComplete.Invoke(this, result);
            }
        }

        private void TriggerGetFilesFailed(Exception e)
        {
            if (GetFilesFailed != null)
            {
                GetFilesFailed.Invoke(this, e);
            }
        }

        #endregion
    }
}

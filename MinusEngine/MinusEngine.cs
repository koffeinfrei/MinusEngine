using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MinusEngine
{

    #region Handler Delegates

    public delegate void oAuthCompleteHandler(MinusEngine sender, oAuthResult result);
    public delegate void oAuthFailedHandler(MinusEngine sender, Exception e);

    public delegate void UploadItemCompleteHandler(MinusEngine sender, FileResult result);
    public delegate void UploadItemFailedHandler(MinusEngine sender, Exception e);

    public delegate void CreateFolderCompleteHandler(MinusEngine sender, FolderResult result);
    public delegate void CreateFolderFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFolderListCompleteHandler(MinusEngine sender, IList<FolderResult> result, PaginationResult pResult);
    public delegate void GetFolderListFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFolderCompleteHandler(MinusEngine sender, FolderResult result);
    public delegate void GetFolderFailedHandler(MinusEngine sender, Exception e);

    public delegate void ModifyFolderCompleteHandler(MinusEngine sender, FolderResult result);
    public delegate void ModifyFolderFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFileCompleteHandler(MinusEngine sender, FileResult result);
    public delegate void GetFileFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFileListCompleteHandler(MinusEngine sender, IList<FileResult> result, PaginationResult pResult);
    public delegate void GetFileListFailedHandler(MinusEngine sender, Exception e);

    public delegate void ModifyFileCompleteHandler(MinusEngine sender, FileResult result);
    public delegate void ModifyFileFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFollowersCompleteHandler(MinusEngine sender, IList<UserResult> result, PaginationResult pResult);
    public delegate void GetFollowersFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetFollowingCompleteHandler(MinusEngine sender, IList<UserResult> result, PaginationResult pResult);
    public delegate void GetFollowingFailedHandler(MinusEngine sender, Exception e);

    public delegate void AddFolloweeCompleteHandler(MinusEngine sender, UserResult result);
    public delegate void AddFolloweeFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetLastMessageCompleteHandler(MinusEngine sender, List<MessageResult> result, PaginationResult pResult);
    public delegate void GetLastMessageFailedHandler(MinusEngine sender, Exception e);

    public delegate void GetMessageCompleteHandler(MinusEngine sender, List<MessageResult> result, PaginationResult pResult);
    public delegate void GetMessageFailedHandler(MinusEngine sender, Exception e);

    public delegate void SendMessageCompleteHandler(MinusEngine sender, MessageResult messageResult);
    public delegate void SendMessageFailedHandler(MinusEngine sender, Exception e);

    #endregion

    public class MinusEngine
    {

        public static readonly String BASE_URL = "https://minus.com/";
        public static readonly Uri AUTH_URL = new Uri(BASE_URL + "oauth/token");
        public static readonly Uri FOLDERS_URL = new Uri(BASE_URL + "api/v2/folders/");
        public static readonly Uri USERS_URL = new Uri(BASE_URL + "api/v2/users/");
        public static readonly Uri FILES_URL = new Uri(BASE_URL + "api/v2/files/");
        public static readonly Uri MESSAGES_URL = new Uri(BASE_URL + "api/v2/activeuser/messages/");
        public static readonly Uri ACTIVE_URL = new Uri(BASE_URL + "api/v2/activeuser/");

        #region Handler Events

        public event oAuthCompleteHandler oAuthComplete;
        public event oAuthFailedHandler oAuthFailed;

        public event UploadItemCompleteHandler UploadItemComplete;
        public event UploadItemFailedHandler UploadItemFailed;

        public event CreateFolderCompleteHandler CreateFolderComplete;
        public event CreateFolderFailedHandler CreateFolderFailed;

        public event GetFolderListCompleteHandler GetFoldersComplete;
        public event GetFolderListFailedHandler GetFoldersFailed;

        public event GetFolderCompleteHandler GetFolderComplete;
        public event GetFolderFailedHandler GetFolderFailed;

        public event ModifyFolderCompleteHandler ModifyFolderComplete;
        public event ModifyFolderFailedHandler ModifyFolderFailed;

        public event GetFileCompleteHandler GetFileComplete;
        public event GetFileFailedHandler GetFileFailed;

        public event GetFileListCompleteHandler GetFilesComplete;
        public event GetFileListFailedHandler GetFilesFailed;

        public event ModifyFileCompleteHandler ModifyFileComplete;
        public event ModifyFileFailedHandler ModifyFileFailed;

        public event GetFollowersCompleteHandler GetFollowersComplete;
        public event GetFollowersFailedHandler GetFollowersFailed;

        public event GetFollowingCompleteHandler GetFollowingComplete;
        public event GetFollowingFailedHandler GetFollowingFailed;

        public event AddFolloweeCompleteHandler AddFolloweeComplete;
        public event AddFolloweeFailedHandler AddFolloweeFailed;

        public event GetLastMessageCompleteHandler GetLastMessageComplete;
        public event GetLastMessageFailedHandler GetLastMessageFailed;

        public event GetMessageCompleteHandler GetMessageComplete;
        public event GetMessageFailedHandler GetMessageFailed;

        public event SendMessageCompleteHandler SendMessageComplete;
        public event SendMessageFailedHandler SendMessageFailed;

        #endregion

        #region Authorization

        public void oAuth(String username, String password, String clientId, String secret, String access)
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
                        client.UploadStringAsync(AUTH_URL, "POST", data.ToString());
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

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
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

        public void oAuthRefresh(String clientId, String secret, String refreshToken, String access)
        {
            StringBuilder data = new StringBuilder();
            data.Append("grant_type=refresh_token&client_id=" + clientId + "&client_secret=" + secret +
                               "&scope=" + access + "&refresh_token=" + refreshToken);

            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadStringAsync(AUTH_URL, "POST", data.ToString());
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

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Refresh oAuth operation failed: " + e.Error.Message);
                    TriggeroAuthFailed(e.Error);
                    return;
                }

                oAuthResult result = JsonConvert.DeserializeObject<oAuthResult>(e.Result);
                Debug.WriteLine("Refresh oAuth operation successful: " + result);
                TriggeroAuthComplete(result);
            };
        }

        #endregion

        #region Folders

        #region Gets

        public void GetFolderList(String username, String accessToken, Int32 pageNum)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFolders = new Uri(USERS_URL + username + "/folders?page=" + pageNum + "&bearer_token=" + accessToken);

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
                        TriggerGetFolderListFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFolderListFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Folder List operation failed: " + e.Error.Message);
                    TriggerGetFolderListFailed(e.Error);
                    return;
                }

                PaginationResult pResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                IList<FolderResult> results = objectResults.Select(objectSearch =>
                    JsonConvert.DeserializeObject<FolderResult>(objectSearch.ToString())).ToList();

                Debug.WriteLine("Get Folder List operation successful: " + results);
                TriggerGetFolderListComplete(results, pResult);
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
                        TriggerGetFolderFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFolderFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Folder operation failed: " + e.Error.Message);
                    TriggerGetFolderFailed(e.Error);
                    return;
                }

                FolderResult result = JsonConvert.DeserializeObject<FolderResult>(e.Result);
                Debug.WriteLine("Get Folder operation successful: " + result);
                TriggerGetFolderComplete(result);
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
                        TriggerModifyFolderFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerModifyFolderFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Modify Folder operation failed: " + e.Error.Message);
                    TriggerModifyFolderFailed(e.Error);
                    return;
                }

                FolderResult result = JsonConvert.DeserializeObject<FolderResult>(e.Result);
                Debug.WriteLine("Modify Folder operation successful: " + result);
                TriggerModifyFolderComplete(result);
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
                        client.UploadStringAsync(createFolder, "POST", data.ToString());
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
                FolderResult result = JsonConvert.DeserializeObject<FolderResult>(e.Result);

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
                        client.DownloadStringAsync(getFolder);
                    }
                    catch (WebException e)
                    {
                        TriggerGetFileFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFileFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get File operation failed: " + e.Error.Message);
                    TriggerGetFileFailed(e.Error);
                    return;
                }

                FileResult result = JsonConvert.DeserializeObject<FileResult>(e.Result);
                Debug.WriteLine("Get File operation successful: " + result);
                TriggerGetFileComplete(result);
            };
        }

        public void GetFileList(String folderID, String accessToken, Int32 pageNum)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFiles = new Uri(FOLDERS_URL + folderID + "/files?page=" + pageNum + "&bearer_token=" + accessToken);

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
                        TriggerGetFileListFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFileListFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get File List operation failed: " + e.Error.Message);
                    TriggerGetFileListFailed(e.Error);
                    return;
                }

                PaginationResult pResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                IList<FileResult> results = objectResults.Select(objectSearch =>
                    JsonConvert.DeserializeObject<FileResult>(objectSearch.ToString())).ToList();

                Debug.WriteLine("Get File List operation successful: " + results);
                TriggerGetFileListComplete(results, pResult);
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
                        TriggerModifyFileFailed(e);
                    }
                }
                    );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerModifyFileFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Modify File operation failed: " + e.Error.Message);
                    TriggerModifyFileFailed(e.Error);
                    return;
                }

                FileResult result = JsonConvert.DeserializeObject<FileResult>(e.Result);
                Debug.WriteLine("Modify File operation successful: " + result);
                TriggerModifyFileComplete(result);
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
            UploadItem(accessToken, folderId, caption, filePath, data);
        }

        public void UploadItem(String accessToken, String folderId, String caption, String filePath, Stream data)
        {
            Uri upload = new Uri(FOLDERS_URL + folderId + "/files?bearer_token=" + accessToken);

            string boundaryString = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(upload);
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundaryString;
                request.Host = "minus.com";
                request.KeepAlive = false;

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
                                             + "name=\"file\"; "
                                             + "filename=\"{0}\""
                                             + "\r\nContent-Type: {1}\r\n\r\n",
                                             fileName,
                                             Path.GetExtension(filePath));

                        byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)data.Length))];
                        postDataWriter.Flush();
                        PushData(data, postDataStream, buffer);
                        data.Close();

                        //Filename
                        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                        postDataWriter.Write("Content-Disposition: form-data;"
                                             + " name=\"filename\"\r\n\r\n{0}",
                                             fileName);

                        postDataWriter.Flush();

                        //Caption
                        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                        postDataWriter.Write("Content-Disposition: form-data;"
                                             + " name=\"caption\"\r\n\r\n{0}",
                                             caption);

                        postDataWriter.Flush();



                        postDataWriter.Write("\r\n--" + boundaryString + "--");
                        postDataWriter.Flush();

                        request.ContentLength = postDataStream.Length;

                        Stream requestStream = request.GetRequestStream();

                        postDataStream.WriteTo(requestStream);
                        postDataStream.Flush();



                        WebResponse response = request.GetResponse();                
                        Stream stream = response.GetResponseStream();

                        if (stream != null)
                        {
                            StreamReader reader = new StreamReader(stream);
                            string responseString = reader.ReadToEnd();
                            FileResult result = JsonConvert.DeserializeObject<FileResult>(responseString);
                            Debug.WriteLine("UploadItem operation successful: " + result);
                            TriggerUploadItemComplete(result);
                        }

                        postDataStream.Close();
                    }
                    catch (Exception e)
                    {

                        Debug.WriteLine("Upload Failed " + e.Message);
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

        #region Users

        #region Gets

        public void GetFollowers(String username, String accessToken, Int32 pageNum)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFollowers = new Uri(USERS_URL + username + "/followers?page=" + pageNum + "&bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getFollowers);
                    }
                    catch (WebException e)
                    {
                        TriggerGetFollowersFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFollowersFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Followers operation failed: " + e.Error.Message);
                    TriggerGetFollowersFailed(e.Error);
                    return;
                }

                
                PaginationResult paginationResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                IList<UserResult> results = objectResults.Select(objectResult =>
                    JsonConvert.DeserializeObject<UserResult>(objectResult.ToString())).ToList();
              
                Debug.WriteLine("Get Followers operation successful: " + results);
                TriggerGetFollowersComplete(results, paginationResult);
            }; 
        }

        public void GetFollowing(String username, String accessToken, Int32 pageNum)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getFollowing = new Uri(USERS_URL + username + "/following?page=" + pageNum + "&bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getFollowing);
                    }
                    catch (WebException e)
                    {
                        TriggerGetFollowingFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetFollowingFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Following operation failed: " + e.Error.Message);
                    TriggerGetFollowingFailed(e.Error);
                    return;
                }

                PaginationResult paginationResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                IList<UserResult> results = objectResults.Select(objectResult =>
                    JsonConvert.DeserializeObject<UserResult>(objectResult.ToString())).ToList();

                Debug.WriteLine("Get Following operation successful: " + results);
                TriggerGetFollowingComplete(results, paginationResult);
            };  
        }

        #endregion

        #region Add

        public void AddFollowee(String username, String accessToken, String followee)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();

            Uri getFollowing = new Uri(USERS_URL + username + "/following?bearer_token=" + accessToken);

            data.Append("slug=" + followee);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadStringAsync(getFollowing, "POST", data.ToString());
                    }
                    catch (WebException e)
                    {
                        TriggerAddFolloweeFailed(e);
                    }
                }
               );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerAddFolloweeFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Add Followee operation failed: " + e.Error.Message);
                    TriggerAddFolloweeFailed(e.Error);
                    return;
                }
                UserResult result = JsonConvert.DeserializeObject<UserResult>(e.Result);

                Debug.WriteLine("Add Followee operation successful: " + result);
                TriggerAddFolloweeComplete(result);
            };
        }

        #endregion

        #endregion

        #region Messages

        #region Gets

        public void GetLastMessage(String accessToken)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getMessage = new Uri(ACTIVE_URL + "messages?bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getMessage);
                    }
                    catch (WebException e)
                    {
                        TriggerGetLastMessageFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetLastMessageFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Last Message operation failed: " + e.Error.Message);
                    TriggerGetLastMessageFailed(e.Error);
                    return;
                }

                PaginationResult pResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                List<MessageResult> results = objectResults.Select(objectResult =>
                    JsonConvert.DeserializeObject<MessageResult>(objectResult.ToString())).ToList();
                Debug.WriteLine("Get Last Message operation successful: " + results);
                TriggerGetLastMessageComplete(results, pResult);
            };
        }

        public void GetMessage(String accessToken, String target, Int32 pageNum)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri getMessage = new Uri(MESSAGES_URL + target + "?page=" + pageNum + "&bearer_token=" + accessToken);

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.DownloadStringAsync(getMessage);
                    }
                    catch (WebException e)
                    {
                        TriggerGetMessageFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerGetMessageFailed(e);
            }

            client.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Get Message operation failed: " + e.Error.Message);
                    TriggerGetMessageFailed(e.Error);
                    return;
                }

                PaginationResult pResult = JsonConvert.DeserializeObject<PaginationResult>(e.Result);
                JObject resultSearch = JObject.Parse(e.Result);
                IList<JToken> objectResults = resultSearch["results"].Children().ToList();
                List<MessageResult> results = objectResults.Select(objectResult =>
                    JsonConvert.DeserializeObject<MessageResult>(objectResult.ToString())).ToList();
                Debug.WriteLine("Get Message operation successful: " + results);
                TriggerGetMessageComplete(results, pResult);
            };
        }

        #endregion

        public void SendMessage(String accessToken, String target, String message)
        {
            CookieAwareWebClient client = new CookieAwareWebClient();
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            Uri sendMessage = new Uri(MESSAGES_URL + target + "?bearer_token=" + accessToken);
            StringBuilder data = new StringBuilder();
            data.Append("body=" + HttpUtility.UrlEncode(message));

            try
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        client.UploadStringAsync(sendMessage, data.ToString());
                    }
                    catch (WebException e)
                    {
                        TriggerSendMessageFailed(e);
                    }
                }
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to submit task to thread pool: " + e.Message);
                TriggerSendMessageFailed(e);
            }

            client.UploadStringCompleted += delegate(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    Debug.WriteLine("Send Message operation failed: " + e.Error.Message);
                    TriggerSendMessageFailed(e.Error);
                    return;
                }

                MessageResult result = JsonConvert.DeserializeObject<MessageResult>(e.Result);
                Debug.WriteLine("Send Message operation successful: " + result);
                TriggerSendMessageComplete(result);
            };
        }

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

        #region oAuth

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

        #endregion
        
        #region Folders

        private void TriggerCreateFolderComplete(FolderResult result)
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


        private void TriggerGetFolderListComplete(IList<FolderResult> result, PaginationResult pResult)
        {
            if (GetFoldersComplete != null)
            {
                GetFoldersComplete.Invoke(this, result, pResult);
            }
        }

        private void TriggerGetFolderListFailed(Exception e)
        {
            if (GetFoldersFailed != null)
            {
                GetFoldersFailed.Invoke(this, e);
            }
        }


        private void TriggerGetFolderComplete(FolderResult result)
        {
            if (GetFolderComplete != null)
            {
                GetFolderComplete.Invoke(this, result);
            }
        }

        private void TriggerGetFolderFailed(Exception e)
        {
            if (GetFolderFailed != null)
            {
                GetFolderFailed.Invoke(this, e);
            }
        }


        private void TriggerModifyFolderComplete(FolderResult result)
        {
            if (ModifyFolderComplete != null)
            {
                ModifyFolderComplete.Invoke(this, result);
            }
        }

        private void TriggerModifyFolderFailed(Exception e)
        {
            if (ModifyFolderFailed != null)
            {
                ModifyFolderFailed.Invoke(this, e);
            }
        }


        #endregion

        #region Files

        private void TriggerUploadItemComplete(FileResult result)
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


        private void TriggerGetFileComplete(FileResult result)
        {
            if (GetFileComplete != null)
            {
                GetFileComplete.Invoke(this, result);
            }
        }

        private void TriggerGetFileFailed(Exception e)
        {
            if (GetFileFailed != null)
            {
                GetFileFailed.Invoke(this, e);
            }
        }


        private void TriggerGetFileListComplete(IList<FileResult> result, PaginationResult pResult)
        {
            if (GetFilesComplete != null)
            {
                GetFilesComplete.Invoke(this, result, pResult);
            }
        }

        private void TriggerGetFileListFailed(Exception e)
        {
            if (GetFilesFailed != null)
            {
                GetFilesFailed.Invoke(this, e);
            }
        }


        private void TriggerModifyFileComplete(FileResult result)
        {
            if (ModifyFileComplete != null)
            {
                ModifyFileComplete.Invoke(this, result);
            }
        }

        private void TriggerModifyFileFailed(Exception e)
        {
            if (ModifyFileFailed != null)
            {
                ModifyFileFailed.Invoke(this, e);
            }
        }

        #endregion
        
        #region Users

        private void TriggerGetFollowersComplete(IList<UserResult> result, PaginationResult paginationResult)
        {
            if (GetFollowersComplete != null)
            {
                GetFollowersComplete.Invoke(this, result, paginationResult);
            }
        }

        private void TriggerGetFollowersFailed(Exception e)
        {
            if (GetFollowersFailed != null)
            {
                GetFollowersFailed.Invoke(this, e);
            }
        }


        private void TriggerGetFollowingComplete(IList<UserResult> result, PaginationResult paginationResult)
        {
            if (GetFollowingComplete != null)
            {
                GetFollowingComplete.Invoke(this, result, paginationResult);
            }
        }

        private void TriggerGetFollowingFailed(Exception e)
        {
            if (GetFollowingFailed != null)
            {
                GetFollowingFailed.Invoke(this, e);
            }
        }


        private void TriggerAddFolloweeComplete(UserResult result)
        {
            if (AddFolloweeComplete != null)
            {
                AddFolloweeComplete.Invoke(this, result);
            }
        }

        private void TriggerAddFolloweeFailed(Exception e)
        {
            if (AddFolloweeFailed != null)
            {
                AddFolloweeFailed.Invoke(this, e);
            }
        }
        
        #endregion       

        #region Messages

        private void TriggerGetLastMessageComplete(List<MessageResult> result, PaginationResult pResult)
        {
            if (GetLastMessageComplete != null)
            {
                GetLastMessageComplete.Invoke(this, result, pResult);
            }
        }

        private void TriggerGetLastMessageFailed(Exception e)
        {
            if (GetLastMessageFailed != null)
            {
                GetLastMessageFailed.Invoke(this, e);
            }
        }


        private void TriggerGetMessageComplete(List<MessageResult> result, PaginationResult pResult)
        {
            if (GetMessageComplete != null)
            {
                GetMessageComplete.Invoke(this, result, pResult);
            }
        }

        private void TriggerGetMessageFailed(Exception e)
        {
            if (GetMessageFailed != null)
            {
                GetMessageFailed.Invoke(this, e);
            }
        }


        private void TriggerSendMessageComplete(MessageResult result)
        {
            if (SendMessageComplete != null)
            {
                SendMessageComplete.Invoke(this, result);
            }
        }

        private void TriggerSendMessageFailed(Exception e)
        {
            if (SendMessageFailed != null)
            {
                SendMessageFailed.Invoke(this, e);
            }
        }

        #endregion
        
        #endregion
    }
}

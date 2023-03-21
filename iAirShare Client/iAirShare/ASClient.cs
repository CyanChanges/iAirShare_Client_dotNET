using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace iAirShare_Client.iAirShare;

public class ASClient : IDisposable
{
    private readonly HttpClient client;
    public string RepositoryId { get; } = "public";

    public ASClient(Uri apiRootUrl, string repoId = "public")
    {
        client = new HttpClient();
        RepositoryId = repoId;
        if (apiRootUrl.ToString().EndsWith('/')) APIRootUri = apiRootUrl;
        APIRootUri = new Uri(apiRootUrl + "/");
    }

    public ASClient(string apiRoot, string repoId = "public") : 
        this(new Uri(apiRoot), repoId) {}

    public Uri APIRootUri { get; }
    public string CurrentPath { get; private set; } = "/";

    public void ChangeDirectory(string path)
    {
        if (path.StartsWith('/'))
        {
            CurrentPath = path;
            return;
        }

        CurrentPath += path.Replace("./", "") + "/";
    }

    public List<ASFile> ListDirectory()
    {
        var d_next = 0;

        var result = new List<ASFile>();

        do
        {
            var requestUri = GetFileUriBuilder("");
            requestUri.Query = $"next={d_next}&count=20";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = requestUri.Uri
            };

            var response = client.Send(request);

            var resultDataTask = response.Content.ReadAsStringAsync();
            resultDataTask.Wait();
            var resultString = resultDataTask.Result;

            var respMediaType = response.Content.Headers.ContentType?.MediaType;
            if (respMediaType != "application/json")
                return null!;

            var resultResponse = JsonConvert
                .DeserializeObject<ASDirResponse>(resultString);

            if (resultResponse?.status != 200)
                return null!;

            if (resultResponse.data?.files != null)
                foreach (var file in resultResponse.data?.files!)
                    if (file?.file_type != null && file.Value.file_type != ASFileType.Null)
                        result.Add(file!.Value);

            d_next = resultResponse.data.HasValue ? resultResponse.data.Value.next : -114514;
        } while (d_next > 0);

        return result;
    }

    public UriBuilder GetFileUriBuilder(string fileName)
    {
        return new UriBuilder(APIRootUri + "file/" + RepositoryId + CurrentPath + fileName);
    }
    
    public byte[] GetFileData(string fileName)
    {
        using var stream = GetFileStream(fileName);
        var buffer = new byte[stream.Length + 10];
        stream.Read(buffer);
        return buffer;
    }

    public void SaveFile(string fileName, string? filePath = null)
    {
        filePath ??= fileName;
        File.WriteAllBytes(filePath, GetFileData(fileName));
    }

    public Stream GetFileStream(string fileName)
    {
        var requestUri = GetFileUriBuilder(fileName);
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri.Uri
        };
        var response = client.Send(request);

        if (response.Content.Headers.ContentType?.MediaType == "application/json")
            return null!;

        return response.Content.ReadAsStream();
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        { 
            client.Dispose();
        }
    }
}
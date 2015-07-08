﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MandrillApi.cs" company="">
//   
// </copyright>
// <summary>
//   Core class for using the MandrillApp Api
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mandrill.Requests;
using Mandrill.Utilities;
using Newtonsoft.Json;

namespace Mandrill
{
  /// <summary>
  ///   Core class for using the MandrillApp Api
  /// </summary>
  public partial class MandrillApi
  {
    #region Fields

    private readonly string baseUrl;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MandrillApi" /> class.
    /// </summary>
    /// <param name="apiKey">
    ///   The API Key recieved from MandrillApp
    /// </param>
    /// <param name="useHttps">
    /// </param>
    /// <param name="timeout">
    ///   Timeout in milliseconds to use for requests.
    /// </param>
    public MandrillApi(string apiKey, bool useHttps = true)
    {
      ApiKey = apiKey;

      if (useHttps)
      {
        baseUrl = Configuration.BASE_SECURE_URL;
      }
      else
      {
        baseUrl = Configuration.BASE_URL;
      }
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   The Api Key for the project received from the MandrillApp website
    /// </summary>
    public string ApiKey { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Execute post to path
    /// </summary>
    /// <param name="path">the path to post to</param>
    /// <param name="data">the payload to send in request body as json</param>
    /// <returns></returns>
    public async Task<T> Post<T>(string path, RequestBase data)
    {
      data.Key = ApiKey;
      try
      {
          using (var client = new HttpClient())
          {
              var response = await
                  client.PostAsync(baseUrl + path,
                      new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"))
                      .ConfigureAwait(false);

              response.EnsureSuccessStatusCode();

             return await response.Content.ReadAsStringAsync().ContinueWith<T>(r => JsonConvert.DeserializeObject<T>(r.Result)).ConfigureAwait(false);
          }
      }
      catch (TimeoutException ex)
      {
        throw new TimeoutException(string.Format("Post timed out to {0}", path));
      }
      catch (HttpRequestException ex)
      {
          throw new MandrillException(string.Format("Post failed {0} with status {1} and content '{2}'", path, ex.ToString(), data));
      }
    }

    #endregion
  }
}
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
using  System.Net.Http.Formatting;
using System.Threading.Tasks;
using Mandrill.Models;
using Mandrill.Requests;
using Mandrill.Utilities;

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
              var response = await client.PostAsJsonAsync(baseUrl + path, data);
              response.EnsureSuccessStatusCode();
              return await response.Content.ReadAsAsync<T>();
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
 public class TencentOAuth2Client : OAuth2Client
    {
        #region Constants and Fields
       
        private const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";

        private const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";
        
        private const string UserInfoEndpoint = "https://graph.qq.com/user/get_user_info";

        private const string OptionIdEndpoint = "https://graph.qq.com/oauth2.0/me";//openid
       
        private readonly string _appId;
        
        private readonly string _appSecret;

        private readonly string[] _requestedScopes;

        private readonly WebProxy _proxy;
        #endregion
        
        public TencentOAuth2Client(string appId, string appSecret, WebProxy proxy)
            : this(appId, appSecret, proxy, "get_user_info") { }

       
        public TencentOAuth2Client(string appId, string appKey, WebProxy proxy, params string[] requestedScopes)
            : base("tencent")
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentNullException("appId");

            if (string.IsNullOrWhiteSpace(appKey))
                throw new ArgumentNullException("appKey");

            if (requestedScopes == null)
                throw new ArgumentNullException("requestedScopes");

            if (requestedScopes.Length == 0)
                throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");
            
            _appId = appId;
            _appSecret = appKey;
            _requestedScopes = requestedScopes;
            _proxy = proxy;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            return BuildUri(AuthorizationEndpoint, new NameValueCollection
                {
                    { "response_type", "code" },
                    { "client_id", Uri.EscapeDataString(_appId) },
                    { "scope", string.Join(", ", _requestedScopes) },
                    { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                    { "state", state },
                });
        }

        private string GetOpenId(string accessToken)
        {

            var uri = BuildUri(OptionIdEndpoint, new NameValueCollection { { "access_token", accessToken } });

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Proxy = _proxy;
            using (var webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return null;

                using (var textReader = new StreamReader(stream))
                {
                    var json = textReader.ReadToEnd();
                    json = json.Substring(json.IndexOf('{'), json.LastIndexOf('}') - json.IndexOf('{') + 1);
                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    // 返回值 callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} );
                    var openid = extraData["openid"].ToString();
                    return openid;
                }
            }
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var openid = GetOpenId(accessToken);
            var uri = BuildUri(UserInfoEndpoint, new NameValueCollection
            {
                { "access_token", accessToken },
                { "oauth_consumer_key", Uri.EscapeDataString(_appId) },
                { "openid", openid },
            });

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.Proxy = _proxy;
            using (var webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return null;

                using (var textReader = new StreamReader(stream))
                {
                    var json = textReader.ReadToEnd();
                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = extraData.ToDictionary(x => x.Key, x => x.Value.ToString());
                    
                    data.Add("id", openid);
                    data.Add("access_token", accessToken);
                    data.Add("name", data["nickname"]);
                    data.Add("picture", data["figureurl_qq_2"]);
                    data["gender"] = data["gender"] == "男" ? "1" : (data["gender"] == "女" ? "2" : "0");
                    return data;
                }
            }
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var uri = BuildUri(TokenEndpoint, new NameValueCollection
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "client_id", _appId },
                    { "client_secret", _appSecret },
                    { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                });

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.Proxy = _proxy;
            using (var webResponse = webRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    var response = reader.ReadToEnd();

                    var results = HttpUtility.ParseQueryString(response);
                    return results["access_token"];
                }
            }
        }

        private static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            var keyValuePairs = queryParameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(queryParameters[k]));
            var qs = String.Join("&", keyValuePairs);

            var builder = new UriBuilder(baseUri) { Query = qs };
            return builder.Uri;
        }

        public static void RewriteRequest()
        {
            var ctx = HttpContext.Current;

            var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
            if (stateString == null || !stateString.Contains("__provider__=tencent"))
                return;

            var q = HttpUtility.ParseQueryString(stateString);
            q.Add(ctx.Request.QueryString);
            q.Remove("state");

            ctx.RewritePath(ctx.Request.Path + "?" + q);
        }
    }

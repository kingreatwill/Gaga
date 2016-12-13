 public  class SinaOAuth2Client : OAuth2Client
    {
        private long _uid;
     
        private const string AuthorizationEndpoint = "https://api.weibo.com/oauth2/authorize";

        private const string TokenEndpoint = "https://api.weibo.com/oauth2/access_token";

        private const string UidEndpoint = "https://api.weibo.com//2/account/get_uid.json";

        private const string UserDataEndpoint = "https://api.weibo.com/2/users/show.json";

        private readonly string _appId;

        private readonly string _appSecret;

        private readonly WebProxy _proxy;

        public SinaOAuth2Client(string appId, string appSecret, WebProxy proxy)
            : base("sina")
        {
            if (appId == null) throw new ArgumentNullException("appId");
            if (appSecret == null) throw new ArgumentNullException("appSecret");

            _appId = appId;
            _appSecret = appSecret;
            _proxy = proxy;
        }
        private static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            var keyValuePairs = queryParameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(queryParameters[k]));
            var qs = String.Join("&", keyValuePairs);

            var builder = new UriBuilder(baseUri) { Query = qs };
            return builder.Uri;
        }
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {

            var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            return BuildUri(AuthorizationEndpoint, new NameValueCollection
                {
                    { "response_type", "code" },
                    { "client_id", Uri.EscapeDataString(_appId) },
                    { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                    { "state", state },
                    { "forcelogin", "true" },
                });
        }

        private string getUid(string accessToken)
        {
            var uri = BuildUri(UidEndpoint, new NameValueCollection
            {
                { "access_token", accessToken },
            });

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Proxy = _proxy;
            using (var webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return "0";

                using (var textReader = new StreamReader(stream))
                {
                    var json = textReader.ReadToEnd();
                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = extraData.ToDictionary(x => x.Key, x => x.Value.ToString());
                    return data["uid"];
                }
            }

        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {

            var uri = BuildUri(UserDataEndpoint, new NameValueCollection
            {
                { "uid",_uid.ToString() },//getUid(accessToken)
                { "access_token", accessToken },
            });

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
                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = extraData.ToDictionary(x => x.Key, x => x.Value.ToString());
                    
                    data.Add("access_token", accessToken);
                    data.Add("username", data["name"]);
                    data.Add("picture", data["profile_image_url"]);
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

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Proxy = _proxy;
            webRequest.Method = "POST";
            using (var webResponse = webRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    var response = reader.ReadToEnd();

                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                   // var results = HttpUtility.ParseQueryString(response);
                    _uid =long.Parse(extraData["uid"].ToString()) ;
                    return extraData["access_token"].ToString();
                }
            }
        }
        public static void RewriteRequest()
        {
            var ctx = HttpContext.Current;

            var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
            if (stateString == null || !stateString.Contains("__provider__=sina"))
                return;

            var q = HttpUtility.ParseQueryString(stateString);
            q.Add(ctx.Request.QueryString);
            q.Remove("state");

            ctx.RewritePath(ctx.Request.Path + "?" + q);
        }

    }

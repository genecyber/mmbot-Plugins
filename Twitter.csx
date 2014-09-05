/**
* <description>
*     Provides triggers for sending outgoing messages to Twitter
* </description>
*
*<commands>
*   `mmbot twittersend` (body) [page] - Sends the msg body to the indicated Twitter page\n
*   `mmbot twitter` (body) - Sends the msg body to all Twitter pages\n
*   `mmbot twitterlast` [page]- Replies with the last post from the indicated Twitter page\n
*   `mmbot twitterundo` [page]- Removes the last post from the indicated Twitter page\n
*</commands>
* 
* <author>
*     shannon@mastercoin.org
* </author>
*
* <configuration>
*    MMBOT_PAGES Id|Name comma separated list of key value pairs 
*    MMBOT_TWITTER_ENDPOINT Zapier URL
* </configuration>
*/

var robot = Require<Robot>();

robot.Respond(@"(twittersend) ", msg => {
    var zapEndpoint1 = robot.GetConfigVariable("MMBOT_TWITTER_OMNIENDPOINT");
    var zapEndpoint2 = robot.GetConfigVariable("MMBOT_TWITTER_MSCENDPOINT");
    var zapEndpoint = "";

	var page = msg.Message.Text.Split(' ').Last();
    var id = "";
    var txt = msg.Message.Text.Replace("mmbot twittersend ",string.Empty).Replace(page,string.Empty);
	
    if (page == "mastercoin")
        zapEndpoint = zapEndpoint2;
    if (page == "omni")
        zapEndpoint = zapEndpoint1;

    var packetString = "{\"Body\":\""+txt+"\"}";
    var packet = JObject.Parse(packetString);

    msg.Send(txt + " sending here: "+ zapEndpoint + " with payload: "+packetString);

    robot.Http(zapEndpoint).Post(packet);
});

robot.Respond(@"(tweet) ", msg => {
    var zapEndpoint1 = robot.GetConfigVariable("MMBOT_TWITTER_OMNIENDPOINT");
    var zapEndpoint2 = robot.GetConfigVariable("MMBOT_TWITTER_MSCENDPOINT");
    var txt = msg.Message.Text.Replace("mmbot tweet ",string.Empty);
    
    var packet = makeZapPacket(txt);
    robot.Http(zapEndpoint1).Post(packet);
    packet = makeZapPacket(txt);
    robot.Http(zapEndpoint2).Post(packet);

    msg.Send(txt + " pushed to both twitter accounts");
});

robot.Respond(@"(twitter-help)", msg =>
{
    msg.Send("`/bot mmbot twittersend [post] [page]` - Sends post text to twitter page specified. \n`/bot mmbot tweet [post] [url] ` Sends post text to all tweet pages.\n EXAMPLE: `/bot mmbot tweet Celebrating our anniversary today.`");
});

public JObject makeZapPacket(string txt) {
    var packetString = "{\"Body\":\""+txt+"\"}";
    return JObject.Parse(packetString);
}


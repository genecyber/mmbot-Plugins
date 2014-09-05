/**
* <description>
*     Provides triggers for sending outgoing messages to Facebook
* </description>
*
*<commands>
*   `mmbot facebooksend` (body) [page] - Sends the msg body to the indicated Facebook page\n
*   `mmbot facebook` (body) - Sends the msg body to all facebook pages\n
*   `mmbot facebooklast` [page]- Replies with the last post from the indicated Facebook page\n
*   `mmbot facebookundo` [page]- Removes the last post from the indicated Facebook page\n
*</commands>
* 
* <author>
*     shannon@mastercoin.org
* </author>
*
* <configuration>
*    MMBOT_PAGES Id|Name comma separated list of key value pairs 
*    MMBOT_ZAPIER_ENDPOINT Zapier URL
* </configuration>
*/

var robot = Require<Robot>();

robot.Respond(@"(facebooksend) ", msg => {
    var zapEndpoint = robot.GetConfigVariable("MMBOT_FACEBOOK_ENDPOINT");
    var pages = robot.GetConfigVariable("MMBOT_FACEBOOK_PAGES").Split(',');

	var page = msg.Message.Text.Split(' ').Last();
    var id = "";
    var txt = msg.Message.Text.Replace("mmbot facebooksend ",string.Empty).Replace(page,string.Empty);
	
    if (page == "mastercoin")
        id = "00000000000000";
    if (page == "omni")
        id = "00000000000000";

    var packetString = "{\"Body\":\""+txt+"\", \"Page\":\""+id+"\"}";
    var packet = JObject.Parse(packetString);

    msg.Send(txt + " sending here: "+ zapEndpoint + " with payload: "+packetString);

    robot.Http(zapEndpoint).Post(packet);
});

robot.Respond(@"(facebook) ", msg => {
    var zapEndpoint = robot.GetConfigVariable("MMBOT_FACEBOOK_ENDPOINT");
    var url = msg.Message.Text.Split(' ').Last();
    if (!url.Contains("http"))
        url = "";
    var txt = msg.Message.Text.Replace("mmbot facebook ",string.Empty).Replace(url,string.Empty);
    
    var packet = makeZapPacket("00000000000000",txt, url);
    robot.Http(zapEndpoint).Post(packet);
    packet = makeZapPacket("00000000000000",txt, url);
    robot.Http(zapEndpoint).Post(packet);

    msg.Send(txt + " "+url+" pushed to both facebook pages");
});

robot.Respond(@"(facebook-help)", msg =>
{
    msg.Send("`/bot mmbot facebooksend [post] [page]` - Sends post text to facebook page specified. \n`/bot mmbot facebook [post] [url] ` Sends post text to all facebook pages.\n EXAMPLE: `/bot mmbot facebook This Is A Great Post http://www.greatpst.com/somePost/`");
});

public JObject makeZapPacket(string id, string txt, string url) {
    var packetString = "{\"Body\":\""+txt+"\", \"Url\":\""+url+"\", \"Page\":\""+id+"\"}";
    return JObject.Parse(packetString);
}


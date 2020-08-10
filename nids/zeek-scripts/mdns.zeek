## Monitors DNS events for mdns traffic.

@load base/protocols/dns/main
@load base/utils/site
@load base/utils/addrs

module mdns;

export 
{
	redef enum Log::ID += { LOG };
	type Info: record 
    {
		ts: time &log;
		## Query with original letter casing
		query: string &log;
		answer: string &log;
	};
}
event zeek_init() &priority=5
{
    # Create the stream. This adds a default filter automatically.
    Log::create_stream(mdns::LOG, [$columns=Info, $path="mdns"]);
}

event dns_PTR_reply(c: connection, msg: dns_msg, ans: dns_answer, name: string)
{
        if (/\.local/ !in name && /\.local/ !in ans$query)
        {
                return;
        }

        local rec: mdns::Info = [$ts=network_time(), $query=ans$query, $answer=name];
        Log::write(mdns::LOG, rec);
}

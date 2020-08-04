@load base/protocols/dns/main
@load base/utils/site
@load base/utils/addrs

module kropotkindns;


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
    Log::create_stream(kropotkindns::LOG, [$columns=Info, $path="dnshosts"]);
}

event dns_AAAA_reply(c: connection, msg: dns_msg, ans: dns_answer, a: addr)
{
	#Check if an answer is local
	if (!Site::is_private_addr(a))
	{
		return;
	}

	local rec: kropotkindns::Info = [$ts=network_time(), $query=ans$query, $answer=addr_to_uri(a)];

	Log::write(kropotkindns::LOG, rec);
}

event dns_A_reply(c: connection, msg: dns_msg, ans: dns_answer, a: addr)
{
	#Check if an answer is local
	if (!Site::is_private_addr(a))
	{
		return;
	}

	local rec: kropotkindns::Info = [$ts=network_time(), $query=ans$query, $answer=addr_to_uri(a)];

	Log::write(kropotkindns::LOG, rec);
}

event dns_PTR_reply(c: connection, msg: dns_msg, ans: dns_answer, name: string)
{
        if (/\.local/ !in name && /\.local/ !in ans$query)
        {
                return;
        }

        local rec: kropotkindns::Info = [$ts=network_time(), $query=ans$query, $answer=name];
        Log::write(kropotkindns::LOG, rec);
}

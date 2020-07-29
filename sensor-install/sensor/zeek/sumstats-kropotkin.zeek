@load base/frameworks/sumstats

event conn_stats (c: connection, os: endpoint_stats, rs: endpoint_stats)
{
	SumStats::observe("conn stats bytes sent", SumStats::Key($id=c$id), SumStats::Observation($num=c$orig$num_bytes_ip));
	SumStats::observe("conn stats bytes recv", SumStats::Key($id=c$id), SumStats::Observation($num=c$resp$num_bytes_ip));
}

event zeek_init ()
{
	local recv_bytes = SumStats::Reducer($stream="conn stats bytes recv", $apply=set(SumStats::SUM));
	local sent_bytes = SumStats::Reducer($stream="conn stats bytes sent", $apply=set(SumStats::SUM));

	SumStats::create([$name="received bytes", $epoch = 1min, $reducers=set(recv_bytes),
			  $epoch_results(ts: time, key: SumStats::Key, result: Sumstats::Result) =
			  {
			  	print fmt("Number of bytes recived: %.0f", result["conn stats bytes recv"]$sum);
			  }]);
}

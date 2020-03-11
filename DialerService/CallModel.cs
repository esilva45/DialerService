namespace ServiceIntegration {
    class CallModel {
        public CallModel() { }

        public CallModel(string url_ligacao, string origem_tel, string destino_tel, string dt_inicio_chamada, string dt_fim_chamada,
            string tempo_conversacao, string campanha, string token, string call_id, string history_id, string reason_terminated,
            string final_dn, string chain, string to_type, string final_type, string from_dispname, string to_dispname, string missed_queue_calls) {
            UrlLigacao = url_ligacao;
            OrigemTel = origem_tel;
            CallId = call_id;
            HistoryId = history_id;
            DestinoTel = destino_tel;
            ReasonTerminated = reason_terminated;
            FinalDn = final_dn;
            Chain = chain;
            DestinationType = to_type;
            FinalDestinationType = final_type;
            SourceDisplayName = from_dispname;
            DestinationDisplayName = to_dispname;
            MissedQueueCalls = missed_queue_calls;
            DtInicioChamada = dt_inicio_chamada;
            DtFimChamada = dt_fim_chamada;
            TempoConversacao = tempo_conversacao;
            Token = token;
            Campanha = campanha;
        }

        public string CallId { get; set; }

        public string HistoryId { get; set; }

        public string Token { get; set; }

        public string UrlLigacao { get; set; }

        public string Campanha { get; set; }

        public string ReasonTerminated { get; set; }

        public string FinalDn { get; set; }

        public string Chain { get; set; }

        public string DestinationType { get; set; }

        public string FinalDestinationType { get; set; }

        public string SourceDisplayName { get; set; }

        public string DestinationDisplayName { get; set; }

        public string MissedQueueCalls { get; set; }

        public string OrigemTel { get; set; }

        public string DestinoTel { get; set; }

        public string DtInicioChamada { get; set; }

        public string DtFimChamada { get; set; }

        public string TempoConversacao { get; set; }
    }
}

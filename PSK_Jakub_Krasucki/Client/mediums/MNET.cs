using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Client.mediums
{
    public class MNET : Medium
    {
        NetRemotingUtil netRemotingUtil;

        public MNET(NetRemotingUtil netRemotingUtil)
        {
            this.netRemotingUtil = netRemotingUtil;
        }
        public override string QA(string request)
        {
            return netRemotingUtil.Command(request);
        }
    }
}

using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mundosE
{
    public class ServerStack : Stack
    {
        private const string SIZE = "t2.micro";

        [Output]
        public Output<string> PublicIP { get; private set; }

        [Output]
        public Output<string> PublicDNS { get; private set; }

        public ServerStack()
        {
            var ami = GetAmi.Invoke(new GetAmiInvokeArgs 
            {
                MostRecent = true,
                Owners = { "137112412989" },
                Filters = { new GetAmiFilterInputArgs { Name = "name", Values = "amzn-ami-hvm-*" } }
            });

            var group = new SecurityGroup("web-secgrp", new SecurityGroupArgs
            {
                Description = "Enable SSH",
                Ingress = 
                {
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 22,
                        ToPort = 22,
                        CidrBlocks = { "0.0.0.0/0" }
                    }
                }
            });

            var server = new Instance("deploy-server", new InstanceArgs 
            {
                InstanceType = SIZE,
                VpcSecurityGroupIds = { group.Id},
                UserData = "",
                KeyName = "devops",
                Ami = ami.Apply(a => a.Id)
            });

            var tag = new Tag("name", new() 
            {
                ResourceId = server.Id,
                Key = "Name",
                Value = "MundosE"
            });

            PublicIP = server.PublicIp;
            PublicDNS = server.PublicDns;
        }
    }
}

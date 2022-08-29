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
                Owners = { "099720109477" },
                Filters = { new GetAmiFilterInputArgs { Name = "name", Values = "ubuntu/images/hvm-ssd/ubuntu-focal-20.04-amd64-server-20220610" } }
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
                    },
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 8000,
                        ToPort = 8000,
                        CidrBlocks = { "0.0.0.0/0" }
                    },
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 9000,
                        ToPort = 9000,
                        CidrBlocks = { "0.0.0.0/0" }
                    },
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 8080,
                        ToPort = 8080,
                        CidrBlocks = { "0.0.0.0/0" }
                    },
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 8443,
                        ToPort = 8443,
                        CidrBlocks = { "0.0.0.0/0" }
                    },
                    new SecurityGroupIngressArgs
                    {
                        Protocol = "tcp",
                        FromPort = 5432,
                        ToPort = 5432,
                        CidrBlocks = { "0.0.0.0/0" }
                    }
                },
                Egress = 
                {
                    new SecurityGroupEgressArgs
                    {
                       Protocol = "-1",
                       ToPort = 0,
                       FromPort=0,
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

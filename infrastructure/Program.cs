using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using System.Collections.Generic;

return await Deployment.RunAsync(() =>
{
   var ami = GetAmi.Invoke(new GetAmiInvokeArgs 
   {
       Owners = { },
       MostRecent = true,
       Filters = {
           new GetAmiFilterArgs 
           {
               Name = "name",
               Values = { "amzn-ami-hvm-*"}
           }
       }
   });

   // Export the name of the bucket
   return new Dictionary<string, object?>
   {
      //["bucketName"] = bucket.Id
   };
});

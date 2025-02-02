/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage, (C) 2021 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Minio.Helper;

/*
 * ReplicationConfiguration class used as a container for replication rules. Max number of rules is 100. Size of configuration allowed is 2MB.
 * Please refer:
 * https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetBucketReplication.html
 * https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutBucketReplication.html
 * https://docs.aws.amazon.com/AmazonS3/latest/API/API_DeleteBucketReplication.html
 */

namespace Minio.DataModel.Replication;

[Serializable]
[XmlRoot(ElementName = "ReplicationConfiguration")]
public class ReplicationConfiguration
{
    public ReplicationConfiguration()
    {
    }

    public ReplicationConfiguration(string role, Collection<ReplicationRule> rules)
    {
        if (string.IsNullOrEmpty(role) || string.IsNullOrWhiteSpace(role))
            throw new ArgumentNullException(nameof(role), nameof(role) + " member cannot be empty.");
        if (rules is null || rules.Count == 0)
            throw new ArgumentNullException(nameof(rules), nameof(rules) + " member cannot be an empty list.");
        if (rules.Count >= 1000)
            throw new ArgumentOutOfRangeException(nameof(rules),
                nameof(rules) + " Count of rules cannot exceed maximum limit of 1000.");

        Role = role;
        Rules = rules;
    }

    [XmlElement("Role")] public string Role { get; set; }

    [XmlElement("Rule")] public Collection<ReplicationRule> Rules { get; set; }

    public string MarshalXML()
    {
        XmlWriter xw = null;

        var str = string.Empty;

        try
        {
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var sw = new StringWriter(CultureInfo.InvariantCulture);

            var xs = new XmlSerializer(typeof(ReplicationConfiguration), "");
            using (xw = XmlWriter.Create(sw, settings))
            {
                xs.Serialize(xw, this, ns);
                xw.Flush();

                str = Utils.RemoveNamespaceInXML(sw.ToString()).Replace("\r", "").Replace("\n", "");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            // throw ex;
        }
        finally
        {
            xw?.Close();
        }

        return str;
    }
}

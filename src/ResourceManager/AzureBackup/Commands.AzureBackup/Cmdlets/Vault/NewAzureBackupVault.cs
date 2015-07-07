﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.AzureBackup.Helpers;
using Microsoft.Azure.Commands.AzureBackup.Models;
using System;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.AzureBackup.Cmdlets
{
    /// <summary>
    /// API to create an azure backup vault in a subscription
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureBackupVault"), OutputType(typeof(AzurePSBackupVault))]
    public class NewAzureBackupVault : AzureBackupCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, HelpMessage = AzureBackupCmdletHelpMessage.ResourceGroupName)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = AzureBackupCmdletHelpMessage.ResourceName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = AzureBackupCmdletHelpMessage.Location)]
        [ValidateNotNullOrEmpty]
        public string Region { get; set; }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.StorageType)]
        public AzureBackupVaultStorageType Storage { get; set; }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.Sku)]
        [ValidateSet("standard")]
        public string Sku { get; set; }

        // TODO: Add support for tags
        //[Alias("Tags")]
        //[Parameter(Mandatory = false, HelpMessage = AzureBackupCmdletHelpMessage.ResourceTags)]
        //public Hashtable[] Tag { get; set; }

        private string DefaultSKU = "standard";

        public override void ExecuteCmdlet()
        {
            ExecutionBlock(() =>
            {
                base.ExecuteCmdlet();
                InitializeAzureBackupCmdlet(ResourceGroupName, Name);

                string skuParam = Sku ?? DefaultSKU;
                WriteDebug(String.Format("Creating backup vault with ResourceGroupName: {0}, ResourceName: {1}", ResourceGroupName, Name));

                var createdVault = AzureBackupClient.CreateOrUpdateAzureBackupVault(ResourceGroupName, Name, Region, skuParam);

                if (Storage != 0)
                {
                    WriteDebug(String.Format("Setting storage type for the resource, Type: {0}", Storage));

                    AzureBackupClient.UpdateStorageType(Storage.ToString());
                }

                WriteObject(VaultHelpers.GetCmdletVault(createdVault, AzureBackupClient.GetStorageTypeDetails(VaultHelpers.GetResourceGroup(createdVault.Id), createdVault.Name)));
            });
        }
    }
}

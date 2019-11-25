using ElectionGuard.SDK.Models.ElectionGuardAPI;
using ElectionGuard.SDK.Serialization;

namespace ElectionGuard.SDK.Models
{
    public class ElectionGuardConfig
    {
        public ElectionGuardConfig()
        {
        }

        /// <summary>
        /// Overloaded constructor initializes going from unmanaged struct format to expected managed C# class
        /// </summary>
        /// <param name="apiConfig"></param>
        public ElectionGuardConfig(ElectionGuardAPI.APIConfig apiConfig)
        {
            NumberOfSelections = (int)apiConfig.NumberOfSelections;
            NumberOfTrustees = (int)apiConfig.NumberOfTrustees;
            Threshold = (int)apiConfig.Threshold;
            SubgroupOrder = (int)apiConfig.SubgroupOrder;
            ElectionMetadata = apiConfig.ElectionMetadata;
            JointPublicKey = ByteSerializer.ConvertToBase64String(apiConfig.SerializedJointPublicKey);
        }

        public int NumberOfSelections { get; set; }
        
        public int NumberOfTrustees { get; set; }

        public int Threshold { get; set; }

        public int SubgroupOrder { get; set; }

        public string ElectionMetadata { get; set; }

        public string JointPublicKey { get; set; }

        /// <summary>
        /// Convert form the managed C# class to the unmanaged api struct
        /// </summary>
        /// <param name="electionGuardConfig"></param>
        /// <returns></returns>
        public ElectionGuardAPI.APIConfig GetApiConfig()
        {
            return new ElectionGuardAPI.APIConfig()
            {
                NumberOfSelections = (uint)NumberOfSelections,
                NumberOfTrustees = (uint)NumberOfTrustees,
                Threshold = (uint)Threshold,
                SubgroupOrder = (uint)SubgroupOrder,
                ElectionMetadata = ElectionMetadata,
                SerializedJointPublicKey = JointPublicKey == null ? new SerializedBytes() : ByteSerializer.ConvertFromBase64String(JointPublicKey),
            };
        }
    }
}
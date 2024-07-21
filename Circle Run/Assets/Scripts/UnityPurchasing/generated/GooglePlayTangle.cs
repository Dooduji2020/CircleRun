// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("TuOZL2ag+TAU7iktltY/PGAxHOp4LVm+Q/1Dnb/U3m+XvioeN7hyqIg6uZqItb6xkj7wPk+1ubm5vbi7CIlcTMnlZ1oPkoyJhF4x/habV3M/k17uRWru1lXkajDjajovzya7J0vM+pJL/DCexVeSorQoCYy5Ds6UoZn8z7wdHD8e3o4I1YGXovKsL3NDj/7cTsk7wJZ7jwiqVilaUJAmv6R/kL7NYZlrS9kaL955QtX1IlPj/HADNKHNrv23HOhLZGuJDneiraAoeRmSAiGRbqC7CcwWeUNNgJuDQzzqwPLmQiiR616tOJeN7WpNDzNXOrm3uIg6ubK6Orm5uHspPC4SBofie+83pNMIkKR/787ZDdcd7IrjHHC9efjGSwsBsbq7ubi5");
        private static int[] order = new int[] { 3,11,3,13,13,12,12,9,8,9,12,13,13,13,14 };
        private static int key = 184;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}

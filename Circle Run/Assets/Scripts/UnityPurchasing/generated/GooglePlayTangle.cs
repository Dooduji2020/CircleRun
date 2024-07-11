// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("TuIvnzQbn6cklRtBkhtLXr5XylZZCGjjc1DgH9HKeL1nCDI88eryMtUO4c+8EOgaOqhrXq8IM6SEUyKSOr2L4zqNQe+0JuPTxVl4/ch/v+X5S8jr+cTPwONPgU8+xMjIyMzJyglcKM8yjDLszqWvHubPW29GyQPZP5LoXhfRiEFln1hc56dOTRFAbZtLyMbJ+UvIw8tLyMjJClhNX2N39tDojb7NbG1Ob6//eaTw5tOD3V4CTZuxg5czWeCaL9xJ5vycGzx+QiYy/o+tP7hKsecK/nnbJ1grIeFXzpMKnkbVonnh1Q6ev6h8pmyd+5JtefgtPbiUFit+4/349S9Aj2fqJgKNAXJF0LzfjMZtmToVGvh/BtPc0QHMCIm3OnpwwMvKyMnI");
        private static int[] order = new int[] { 2,6,8,12,8,11,12,12,11,11,11,11,13,13,14 };
        private static int key = 201;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}

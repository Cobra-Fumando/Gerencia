namespace Serviços.Config
{
    public class GerarNumero
    {
        public int[] numero { get; }
        public GerarNumero()
        {
            numero = new int[5];
            for (int i = 0; i < 5; i++)
            {
                numero[i] = Random.Shared.Next(0, 10);
            }
        }
    }
}

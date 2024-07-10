using System;
using System.Threading;

namespace DonutC
{
    class Donut
    {
        static void Main()
        {
            RunAnimation();
        }

        static void RunAnimation()
        {
            const int screenWidth = 150; // Ampliamos la anchura de la consola
            const int screenHeight = 40; // Ampliamos la altura de la consola
            const double K1 = 100; // Factor de escala para ajustar el tamaño del donut
            const double K2 = 15; // Ajustamos K2 para posicionar mejor el donut
            const double thetaSpacing = 0.07;
            const double phiSpacing = 0.02;

            double A = 1, B = 1;

            char[] luminanceChars = ".,-~:;=!*#$@".ToCharArray();
            char[] output = new char[screenWidth * screenHeight];
            double[] zbuffer = new double[screenWidth * screenHeight];

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            try
            {
                Console.SetWindowSize(screenWidth, screenHeight + 1); // +1 para evitar problemas de línea final
                Console.SetBufferSize(screenWidth, screenHeight + 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting console window/buffer size: " + ex.Message);
                return;
            }

            while (true)
            {
                Array.Fill(output, ' ');
                Array.Fill(zbuffer, 0);

                double cosA = Math.Cos(A), sinA = Math.Sin(A);
                double cosB = Math.Cos(B), sinB = Math.Sin(B);

                for (double theta = 0; theta < 2 * Math.PI; theta += thetaSpacing)
                {
                    double cosTheta = Math.Cos(theta), sinTheta = Math.Sin(theta);

                    for (double phi = 0; phi < 2 * Math.PI; phi += phiSpacing)
                    {
                        double cosPhi = Math.Cos(phi), sinPhi = Math.Sin(phi);

                        double circlex = cosTheta + 2; // R1 + R2*cos(theta)
                        double circley = sinTheta;

                        double x = circlex * (cosB * cosPhi + sinA * sinB * sinPhi) - circley * cosA * sinB;
                        double y = circlex * (sinB * cosPhi - sinA * cosB * sinPhi) + circley * cosA * cosB;
                        double z = K2 + cosA * circlex * sinPhi + circley * sinA;
                        double ooz = 1 / z;

                        int xp = (int)(screenWidth / 2 + K1 * ooz * x);
                        int yp = (int)(screenHeight / 2 - K1 * ooz * y);

                        double L = cosPhi * cosTheta * sinB - cosA * cosTheta * sinPhi - sinA * sinTheta + cosB * (cosA * sinTheta - cosTheta * sinA * sinPhi);

                        if (L > 0 && xp >= 0 && xp < screenWidth && yp >= 0 && yp < screenHeight)
                        {
                            int index = xp + screenWidth * yp;
                            if (ooz > zbuffer[index])
                            {
                                zbuffer[index] = ooz;
                                int luminanceIndex = (int)(L * 8);
                                if (luminanceIndex >= 0 && luminanceIndex < luminanceChars.Length)
                                {
                                    output[index] = luminanceChars[luminanceIndex];
                                }
                            }
                        }
                    }
                }

                // Llevar el cursor a la posición de inicio de la consola
                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < screenHeight; i++)
                {
                    for (int j = 0; j < screenWidth; j++)
                    {
                        Console.Write(output[j + screenWidth * i]);
                    }
                    Console.WriteLine();
                }

                // Actualización de ángulos para la animación
                A += 0.07;
                B += 0.03;

                // Pausa breve para controlar la velocidad de la animación
                Thread.Sleep(50);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class IndemnizacionesData
    {
        public string Descripcion { get; set; }
        public int Valor { get; set; }
        public int Index { get; set; }

        public List<IndemnizacionesData> GetIndemnizaciones()
        {
            return new List<IndemnizacionesData>()
            {
                new IndemnizacionesData(){ Index = 1, Descripcion = "Pérdida total e irreparable de ambos ojos", Valor = 100 },
                new IndemnizacionesData(){ Index = 2,Descripcion = "Pérdida total e irreparable de ambas manos o ambos pies", Valor = 100 },
                new IndemnizacionesData(){ Index = 3,Descripcion = "Sordera Total bilateral", Valor = 100 },
                new IndemnizacionesData(){Index = 4, Descripcion = "Perdida del habla", Valor = 100 },
                new IndemnizacionesData(){ Index = 5,Descripcion = "Pérdida o inutilización de una mano y de un pie", Valor = 100 },
                new IndemnizacionesData(){ Index = 6,Descripcion = "Pérdida o inutilización de una mano o un pie y la visión de un ojo", Valor = 100 },
                new IndemnizacionesData(){ Index = 7,Descripcion = "Pérdida o inutilización del brazo de la mano derecha", Valor = 60 },
                new IndemnizacionesData(){ Index = 8,Descripcion = "Perdida completa de la visión de un ojo", Valor = 60 },
                new IndemnizacionesData(){ Index = 9,Descripcion = "Sordera Total Unilateral", Valor = 50 },
                new IndemnizacionesData(){ Index = 10,Descripcion = "Pérdida o inutilización de una sola mano o de un pie", Valor = 60 },
                new IndemnizacionesData(){ Index = 11,Descripcion = "Pérdida o inutilización de una pierna por encima de la rodilla", Valor = 50 },
                new IndemnizacionesData(){Index = 12, Descripcion = "Pérdida completa o inutilización del uso de la cadera", Valor = 30 },
                new IndemnizacionesData(){ Index = 13,Descripcion = "Pérdida o inutilización del dedo pulgar derecho", Valor = 25 },
                new IndemnizacionesData(){ Index = 14,Descripcion = "Pérdida total o inutilización de tres dedos de la mano derecha o el pulgar y otro dedo que no sea el indice", Valor = 20 },
                new IndemnizacionesData(){ Index = 15,Descripcion = "Pérdida o inutilización de una mano", Valor = 20 },
                new IndemnizacionesData(){ Index = 16,Descripcion = "Pérdida o inutilización del dedo pulgar izquierdo", Valor = 20 },
                new IndemnizacionesData(){ Index = 17,Descripcion = "Pérdida completa del uso de la muñeca o del codo derecho", Valor = 20 },
                new IndemnizacionesData(){ Index = 18,Descripcion = "Pérdida completa del uso de algúna rodilla", Valor = 20 },
                new IndemnizacionesData(){ Index = 19,Descripcion = "Fractura no consolidada de una rodilla", Valor = 20 },
                new IndemnizacionesData(){ Index = 20,Descripcion = "Pérdida o inutilización del dedo indice derecho", Valor = 15 },
                new IndemnizacionesData(){ Index = 21,Descripcion = "Pérdida completa del uso de la muñeca o del codo izquierdo", Valor = 15 },
                new IndemnizacionesData(){ Index = 22,Descripcion = "Pérdida completa del uso del tobillo", Valor = 15 },
                new IndemnizacionesData(){ Index = 23,Descripcion = "Pérdida o inutilización del dedo indice izquierdo", Valor = 15 },
                new IndemnizacionesData(){ Index = 24,Descripcion = "Pérdida o inutilización de uno cualquiera de los restantes dedos de la mano o de los pies, siempre que comprenda la totalidad de las falanges de cada uno", Valor = 10 }
            };
        }

        public List<IndemnizacionesData> GetInutilizaciones()
        {
            return new List<IndemnizacionesData>()
            {
                new IndemnizacionesData(){ Index = 1,Descripcion = "Enajenación mental incurable con impotencia funcional absoluta", Valor = 100 },
                new IndemnizacionesData(){ Index = 2,Descripcion = "Parálisis o Invalidez Total y Permanente", Valor = 100 },
                new IndemnizacionesData(){ Index = 3,Descripcion = "Ceguera completa en ambos ojos", Valor = 100 },
                new IndemnizacionesData(){ Index = 4,Descripcion = "Pérdida total e irreparable de ambas manos o ambos pies", Valor = 100 },
                new IndemnizacionesData(){ Index = 5,Descripcion = "Sordera Total bilateral", Valor = 100 },
                new IndemnizacionesData(){ Index = 6,Descripcion = "Perdida del habla", Valor = 100 },
                new IndemnizacionesData(){ Index = 7,Descripcion = "Perdida del brazo o de la mano derecha", Valor = 60 },
                new IndemnizacionesData(){ Index = 8,Descripcion = "Perdida completa de la visión de un ojo", Valor = 50 },
                new IndemnizacionesData(){ Index = 9,Descripcion = "Sordera Total Unilateral", Valor = 50 },
                new IndemnizacionesData(){ Index = 10,Descripcion = "Perdida del Brazo o de la Mano Izquierda", Valor = 50 },
                new IndemnizacionesData(){ Index = 11,Descripcion = "Perdida de una pierna por encima de la rodilla", Valor = 50 },
                new IndemnizacionesData(){ Index = 12,Descripcion = "Perdida de un pie", Valor = 40 },
                new IndemnizacionesData(){ Index = 13,Descripcion = "Perdida completa del uso de la cadera", Valor = 30 },
                new IndemnizacionesData(){ Index = 14,Descripcion = "Fractura no consolidada de una pierna", Valor = 30 },
                new IndemnizacionesData(){ Index = 15,Descripcion = "Perdida del dedo pulgar izquierdo", Valor = 25 },
                new IndemnizacionesData(){ Index = 16,Descripcion = "Pérdida Total de tres dedos de la mano derecha o él pulgar y otro dedo que sea el índice", Valor = 25 },
                new IndemnizacionesData(){ Index = 17,Descripcion = "Perdida completa del uso del hombro derecho", Valor = 25 },
                new IndemnizacionesData(){ Index = 18,Descripcion = "Trastornos en la masticación y el habla", Valor = 25 },
                new IndemnizacionesData(){ Index = 19,Descripcion = "Perdida del dedo pulgar izquierdo", Valor = 20 },
                new IndemnizacionesData(){ Index = 20,Descripcion = "Perdida Total de tres dedos de la mano izquierda o él pulgar y otro dedo que no sea el índice", Valor = 20 },
                new IndemnizacionesData(){ Index = 21,Descripcion = "Perdida completa del uso de la muñeca o del codo derecho", Valor = 20 },
                new IndemnizacionesData(){ Index = 22,Descripcion = "Perdida completa del uso de alguna rodilla", Valor = 20 },
                new IndemnizacionesData(){ Index = 23,Descripcion = "Fractura no consolidada de una rodilla", Valor = 20 },
                new IndemnizacionesData(){ Index = 24,Descripcion = "Perdida del dedo índice derecho", Valor = 15 },
                new IndemnizacionesData(){ Index = 25,Descripcion = "Perdida completa del uso de la muñeca o del codo Izquierdo", Valor = 15 },
                new IndemnizacionesData(){ Index = 26,Descripcion = "Perdida completa del uso del tobillo", Valor = 15 },
                new IndemnizacionesData(){ Index = 27,Descripcion = "Perdida del dedo índice izquierdo", Valor = 12 },
                new IndemnizacionesData(){ Index = 28,Descripcion = "Perdida del dedo anular derecho", Valor = 10 },
                new IndemnizacionesData(){ Index = 29,Descripcion = "Perdida del dedo medio derecho", Valor = 10 },
                new IndemnizacionesData(){ Index = 30,Descripcion = "Perdida del dedo anular izquierdo", Valor = 8 },
                new IndemnizacionesData(){ Index = 31,Descripcion = "Perdida del dedo medio izquierdo", Valor = 8 },
                new IndemnizacionesData(){ Index = 32,Descripcion = "Perdida del dedo gordo de alguno de los pies", Valor = 8 },
                new IndemnizacionesData(){ Index = 33,Descripcion = "Perdida del dedo meñique derecho", Valor = 7 },
                new IndemnizacionesData(){ Index = 34,Descripcion = "Perdida del dedo meñique izquierdo", Valor = 5 }
            };
        }
    }
}

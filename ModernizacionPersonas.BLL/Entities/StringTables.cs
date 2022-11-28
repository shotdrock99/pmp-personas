using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class stringTable
    {
        public string column3 { get; set; }
        public string column2 { get; set; }
        public string column1 { get; set; }

        public List<stringTable> GetGastosTraslado()
        {
            return new List<stringTable>()
            {
                new stringTable(){ column1 = "Si, La condición clínica del paciente necesita maniobras de reanimación porque no pueda respirar, esté inconsciente, esté perdiendo sangre de manera masiva o haya perdido un miembro u órgano.", column2 = "Requiere el traslado en ambulancia medicalizada", column3 = "Trauma craneoencefálico con pérdida de la conciencia, aunque la misma no sea prolongada, trauma en tórax que restrinja la respiración, heridas profundas o extensas con difícil control del sangrado, avulsión de un miembro o parte de él." },
                new stringTable(){ column1 = "Si, se hace evidente que el paciente se esté deteriorando desde el momento en el que ocurrió el accidente, hasta el momento en el que se está recibiendo la llamada, o si el dolor manifestado por el mismo es de carácter insoportable.", column2 = "Requiere traslado en ambulancia básica", column3 = "Trauma craneoencefálico que no mostro sintomatología desde el principio, pero que se manifestó en el transcurso de los minutos subsiguientes al accidente, dolor que aumenta de manera importante, deformidades que se hacen más notorias a medida que pasa el tiempo." },
                new stringTable(){ column1 = "Si, La condición clínica del paciente requiere de medidas diagnósticas y terapéuticas en un servicio de urgencias, pero se encuentra estable, aunque su situación puede empeorar si no se actúa.", column2 = "Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.", column3 = "Accidentes menores en los que el paciente no pierde la conciencia y mantiene una actitud que se modifica, solo en función del trauma. El paciente se queja, pero se encuentra controlado en espera de la atención definitiva." },
                new stringTable(){ column1 = "Si el paciente presenta condiciones médicas que no comprometen su estado general, ni representan un riesgo evidente para la vida o pérdida de miembro u órgano. No obstante, existen riesgos de complicación o secuelas de la enfermedad o lesión si no recibe la atención correspondiente.", column2 = "Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.", column3 = "Accidentes leves, que requieren la prestación de primeros auxilios, pero que no interfieren con la funcionalidad general del paciente" },
                new stringTable(){ column1 = "Si, El paciente presenta una condición clínica relacionada con problemas agudos o crónicos sin evidencia de deterioro que comprometa el estado general de paciente y no representa un riesgo evidente para la vida o la funcionalidad de miembro u órgano.", column2 = "Paciente puede trasladarse en un medio diferente a la ambulancia, siempre y cuando se encuentre acompañado de un adulto y el medio definido, se encuentre en condiciones aptas para ello.", column3 = "Situaciones en las que el paciente, requiere atención, pero esta, es susceptible de ser postergada y manejada en un entorno diferente al de un servicio de urgencias." }
            };
        }


    }
}

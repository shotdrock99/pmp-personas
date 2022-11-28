using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public abstract class CotizacionCompleteness
    {
        protected Cotizacion cotizacion;
        protected CotizacionState currentState;
        protected CotizacionState previousState;
        protected CotizacionState nextState;
        protected bool canIssue;

        public Cotizacion Cotizacion
        {
            get { return cotizacion; }
            set { Cotizacion = value; }
        }

        public CotizacionState CurrentState
        {
            get { return currentState; }
            set { currentState = value; }
        }

        public abstract void NextState();
        public abstract void PreviousState();
        public abstract void CompletenessCheck();
    }
}

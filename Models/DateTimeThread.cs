using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Boganbefaling_eksamen
{
    public class DateTimeThread //tråd der viser dato og tidspunkt
    {
        private Thread _thread;
        private bool _isRunning;

        public DateTimeThread()
        {
            _thread = new Thread(UpdateDateTime);
            _isRunning = false;
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _thread.Start();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _thread.Join();
        }

        private void UpdateDateTime()
        {
            while (_isRunning)
            {
                DateTime now = DateTime.Now;
                Console.WriteLine($"Nuværende dato og tidspunkt: {now}");
                Thread.Sleep( 1000 );
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MemoryLeak.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace testwebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagScenarioController : ControllerBase
    {
        
        public DiagScenarioController()
        {
            Interlocked.Increment(ref DiagnosticsController.Requests);
        }

        private static Processor p = new Processor();

        [HttpGet]
        [Route("memleak/{kb}")]
        public ActionResult<string> memleak(int kb)
        {

            int it = (kb*1000) / 100; 
            for(int i=0; i<it; i++)
            {
                p.ProcessTransaction(new Customer(Guid.NewGuid().ToString()));
            }

            return "success:memleak";
        }
        
        [HttpGet]
        [Route("nomemleak/{kb}")]
        public ActionResult<string> nomemleak(int kb)
        {
            p.Clear();

            return "success:nomemleak";
        }

        [HttpGet]
        [Route("exception")]
        public ActionResult<string> exception()
        {

            throw new Exception("bad, bad code");
        }


        [HttpGet]
        [Route("highcpu/{milliseconds}")]
        public ActionResult<string> highcpu(int milliseconds)
        {
            Stopwatch watch=new Stopwatch();
            watch.Start();

            while (true)
            {
                 watch.Stop();
                 if(watch.ElapsedMilliseconds > milliseconds)
                     break;
                 watch.Start();
            }


            return "success:highcpu";
        }

    }

    class Customer
    {
        private string id;

        public Customer(string id)
        {
            this.id = id;
        }
    }

    class CustomerCache
    {
        private List<Customer> cache = new List<Customer>();

        public void AddCustomer(Customer c)
        {
            cache.Add(c);
        }
        
        public void Clear()
        {
            // cache = null;
            // cache = new List<Customer>();Customer
           cache.Clear();
           cache.TrimExcess();
           // GC.Collect();
        }
    }

    class Processor
    {
        private CustomerCache cache = new CustomerCache();

        public void ProcessTransaction(Customer customer)
        {
            cache.AddCustomer(customer);
        }
        
        public void Clear()
        {
            cache.Clear();
        }
    }


}
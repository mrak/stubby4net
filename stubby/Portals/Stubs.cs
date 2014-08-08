﻿using System;
using System.IO;
using System.Net;
using stubby.Domain;
using utils = stubby.Portals.PortalUtils;

namespace stubby.Portals {

    internal class Stubs : IDisposable {
        private const string Name = "stubs";
        private const string UnregisteredEndoint = "is not a registered endpoint";
        private const string UnexpectedError = "unexpectedtly generated a server error";
        private readonly EndpointDb _endpointDb;
        private readonly HttpListener _listener;
    	private readonly InvocationDb _invocationDb;

		public Stubs(EndpointDb endpointDb, InvocationDb invocationDb)
			: this(endpointDb, invocationDb, new HttpListener())
		{
        }

        public Stubs(EndpointDb endpointDb, InvocationDb invocationDb, HttpListener listener) {
            _endpointDb = endpointDb;
            _listener = listener;
        	_invocationDb = invocationDb;
        }

        public void Dispose() {
            _listener.Stop();
        }

        public void Stop() {
            _listener.Stop();
        }

        public void Start(string location, uint port, uint httpsPort) {
            _listener.Prefixes.Add(utils.BuildUri(location, port));
            _listener.Prefixes.Add(utils.BuildUri(location, httpsPort, true));

            _listener.Start();
            _listener.BeginGetContext(AsyncHandler, _listener);

            utils.PrintListening(Name, location, port);
            utils.PrintListening(Name, location, httpsPort);
        }

		private void ResponseHandler(HttpListenerContext context)
		{
			var incoming = context.ToEndpoint();
			var found = _endpointDb.Find(incoming);

			if (found == null)
			{
				context.Response.StatusCode = (int) HttpStatusCode.NotFound;
				utils.PrintOutgoing(Name, context, UnregisteredEndoint);
				return;
			}

			if (found.Latency > 0)
				System.Threading.Thread.Sleep((int) found.Latency);

			context.Response.StatusCode = found.Status;
			context.Response.Headers.Add(found.Headers);
			WriteResponseBody(context, found);
			utils.PrintOutgoing(Name, context);

			_invocationDb.Add(incoming.ToInvocation());
		}

    	private static void WriteResponseBody(HttpListenerContext context, Response found) {
            string body;

            try {
                body = File.ReadAllText(found.File).TrimEnd(new[]
                {
                    ' ',
                    '\n',
                    '\r',
                    '\t'
                });
            } catch {
                body = found.Body;
            }

            utils.WriteBody(context, body);
        }

        private void AsyncHandler(IAsyncResult result) {
            HttpListenerContext context;
            try {
                context = _listener.EndGetContext(result);
            } catch(HttpListenerException) {
                return;
            }

            utils.PrintIncoming(Name, context);
            utils.SetServerHeader(context);

            try {
                ResponseHandler(context);
            } catch {
                utils.SetStatus(context, HttpStatusCode.InternalServerError);
                utils.PrintOutgoing(Name, context, UnexpectedError);
            }

            context.Response.Close();
            _listener.BeginGetContext(AsyncHandler, _listener);
        }
    }
}
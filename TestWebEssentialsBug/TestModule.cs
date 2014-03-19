using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TestWebEssentialsBug
{
    /// <summary>
    /// Removes CSS from rendered HTML page.
    /// </summary>
    public class TestModule : IHttpModule
    {

        #region IHttpModule Members

        void IHttpModule.Dispose()
        {
            // Nothing to dispose; 
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        #endregion

        static void context_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            app.Response.Filter = new TestFilter(app.Response.Filter);
        }

        private class TestFilter : Stream
        {
            private static readonly Regex eof = new Regex("</html>", RegexOptions.IgnoreCase);

            public TestFilter(Stream sink)
            {
                _sink = sink;
                _builder = new StringBuilder();
            }

            private readonly Stream _sink;
            private StringBuilder _builder;

            #region Properites

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                _sink.Flush();
            }

            public override long Length
            {
                get { return 0; }
            }

            public override long Position { get; set; }

            #endregion

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _sink.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _sink.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _sink.SetLength(value);
            }

            public override void Close()
            {
                _sink.Close();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                var html = System.Text.Encoding.Default.GetString(buffer);

                _builder.Append(html);

                if (!eof.IsMatch(html))
                {
                    //buffer is not complete.  wait until next time to transform HTML.    
                    return;
                }

                var transformed = _builder.ToString();

                var outdata = Encoding.Default.GetBytes(transformed);

                _sink.Write(outdata, 0, outdata.GetLength(0));
            }
        }

    }
}

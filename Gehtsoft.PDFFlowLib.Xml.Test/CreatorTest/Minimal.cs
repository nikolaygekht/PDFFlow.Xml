using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Gehtsoft.PDFFlow.Builder;
using Xunit;

namespace Gehtsoft.PDFFlowLib.Xml.Test.CreatorTest
{
    public sealed class Minimal : IDisposable
    {
        public ConcurrentQueue<string> mTemporaryFiles = new ConcurrentQueue<string>();

        private string TemporateFileName
        {
            get
            {
                var tfn = Path.GetTempFileName();
                mTemporaryFiles.Enqueue(tfn);
                return tfn;
            }
        }

        void IDisposable.Dispose()
        {
            while (!mTemporaryFiles.IsEmpty && mTemporaryFiles.TryDequeue(out string s))
            {
                if (File.Exists(s))
                    File.Delete(s);
            }
        }

        [Fact]
        public void FailOnCallingSaveFirst()
        {
            var creator = new Creator();
            ((Action)(() => creator.Save(TemporateFileName))).Should().Throw<InvalidOperationException>();
        }
    }
}

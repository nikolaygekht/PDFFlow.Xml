using System;
using Gehtsoft.PDFFlow.Models.Shared;

namespace Gehtsoft.PDFFlowLib.Xml.Test
{
    internal static class BasicExtensions
    {
        public static bool ApproximatesTo(this float v, float e, float delta = 1e-5f) => Math.Abs(v - e) < delta;

        public static bool IsWithinMMFrom(this float v, float e) => ApproximatesTo(v, e, 0.283465f);

        public static bool IsWithinMMFrom(this XUnit v, XUnit e) => ApproximatesTo(v.Point, e.Point, 0.283465f);
    }
}

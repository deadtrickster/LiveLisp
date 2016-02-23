using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveLisp.Core.Runtime;
using LiveLisp.Core.Compiler;
using LiveLisp.Core.Types;

namespace LiveLisp.Core.BuiltIns.Streams
{
    [BuiltinsContainer("COMMON-LISP")]
    public static class StreamsDictionary
    {
        [Builtin("input-stream-p", Predicate = true)]
        public static object InputStreamP(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("output-stream-p", Predicate = true)]
        public static object OutputStreamP(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("interacitve-stream-p", Predicate = true)]
        public static object InteractiveStreamP(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("open-stream-p", Predicate = true)]
        public static object OpenStreamP(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("stream-element-type")]
        public static object StreamElementType(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object Streamp(object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin("read-byte")]
        public static object ReadByte(object stream, [Optional] object eof_error_p, [Optional] object eof_value)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-byte")]
        public static object WriteByte(object _byte, object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("peek-char")]
        public static object PeekChar([Optional] object peek_type, [Optional] object input_stream, [Optional] object eof_error_p, [Optional] object eof_value, [Optional] object recursive_p)
        {
            throw new NotImplementedException();
        }

        [Builtin("read-char")]
        public static object ReadChar([Optional] object input_stream, [Optional] object eof_error_p, [Optional] object eof_value, [Optional] object recursive_p)
        {
            throw new NotImplementedException();
        }

        [Builtin("read-char-no-hang")]
        public static object ReadCharNoHang([Optional] object input_stream, [Optional] object eof_error_p, [Optional] object eof_value, [Optional] object recursive_p)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static void Terpri([Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("fresh-line")]
        public static void FreshLine([Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("unread-char")]
        public static void UnreadChar(object character, [Optional] object input_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-char")]
        public static object WriteChar(object character, [Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("read-line")]
        public static object ReadLine([Optional] object input_stream, [Optional] object eof_error_p, [Optional] object eof_value, [Optional] object recursive_p)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-string")]
        public static object WriteString(object str, [Optional] object output_stream, [Key(DefaultValue = "0")] object start, [Key] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-line")]
        public static object WriteLine(object str, [Optional] object output_stream, [Key(DefaultValue = "0")] object start, [Key] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("read-sequence")]
        public static object ReadSequence(object sequence, [Optional] object stream, [Key(DefaultValue = "0")] object start, [Key] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("write-sequence")]
        public static object WriteSequence(object sequence, [Optional] object stream, [Key(DefaultValue = "0")] object start, [Key] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-length")]
        public static object FileLength(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-position")]
        public static object FilePosition(object stream, [Optional] object poisition_spec)
        {
            throw new NotImplementedException();
        }

        [Builtin("file-string-length")]
        public static object FileStringLength(object stream, object obj)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Open(object filespec, [Key] object direction, [Key("ELEMENT-TYPE")] object element_type, [Key] object if_exists, [Key] object if_does_not_exist, [Key] object external_format)
        {
            throw new NotImplementedException();
        }

        [Builtin("stream-external-format")]
        public static object StreamExternalFormat(object stream)
        {
            throw new NotImplementedException();
        }

        [Builtin]
        public static object Close(object stream, [Key] object abort)
        {
            throw new NotImplementedException();
        }

        [Builtin(Predicate=true)]
        public static object Listen([Optional] object input_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("clear-input")]
        public static void ClearInput([Optional] object input_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("finish-output")]
        public static void FinishOutput([Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("force-output")]
        public static void ForceOutput([Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("clear-output")]
        public static void ClearOutput([Optional] object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("y-or-n-p", Predicate = true)]
        public static object YOrNP([Optional] object control, [Rest] object arguments)
        {
            throw new NotImplementedException();
        }

        [Builtin("yes-or-no-p", Predicate = true)]
        public static object YesOrNoP([Optional] object control, [Rest] object arguments)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-synonym-stream")]
        public static object MakeSynonymStream(object symbol)
        {
            throw new NotImplementedException();
        }

        [Builtin("sysnonym-stream-symbol")]
        public static object SynonymStreamSymbol(object synonym_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("broadcast-stream-streams")]
        public static object BroadcastStreamStreams(object broadcast_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-broadcast-stream")]
        public static object MakeBroadcastStream([Rest] object streams)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-two-way-stream")]
        public static object MakeTwoWayStream(object input_stream, object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("two-way-stream-input-stream")]
        public static object TwoWaySreamInputStream(object two_way_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("two-way-stream-output-stream")]
        public static object TwoWaySreamOutputStream(object two_way_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("echo-stream-input-stream")]
        public static object EchoStreamInputStream(object echo_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("echo-stream-output-stream")]
        public static object EchoStreamOutputStream(object echo_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-echo-stream")]
        public static object MakeEchoStream(object input_stream, object output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("concatenated-stream-streams")]
        public static object ConcatenatedStreamStreams(object concatenated_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-concatenated-stream")]
        public static object MakeConcatenatedStream([Rest] object input_streams)
        {
            throw new NotImplementedException();
        }

        [Builtin("get-output-stream-string")]
        public static object GetOutputStreamString(object string_output_stream)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-string-input-stream")]
        public static object MakeStringInputStream(object str, [Optional(0)] object start, [Optional] object end)
        {
            throw new NotImplementedException();
        }

        [Builtin("make-string-output-stream")]
        public static object MakeStringOutputStream([Key] object element_type)
        {
            throw new NotImplementedException();
        }

        [Builtin("stream-error-stream")]
        public static object StreamErrorStream(object condition)
        {
            throw new NotImplementedException();
        }
    }
}

namespace LiveLisp.Core
{
    public partial class DefinedSymbols
    {
        public static Symbol _Debug_Io_;
        public static Symbol _Error_Output_;
        public static Symbol _Query_Io_;
        public static Symbol _Standard_Input_;
        public static Symbol _Standard_Output_;
        public static Symbol _Trace_Output_;
        public static Symbol _Terminal_Io_;



        public static void InitializeStreamsSymbols()
        {
            _Standard_Input_ = cl.Intern("*STANDARD-INPUT*", true);
            _Standard_Output_ = cl.Intern("*STANDARD-OUTPUT*", true);
            _Debug_Io_ = cl.Intern("*DEBUG-IO*", true);
        }
    }
}
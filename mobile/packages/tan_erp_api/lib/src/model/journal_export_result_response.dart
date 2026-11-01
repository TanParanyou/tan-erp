//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/journal_export_line_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'journal_export_result_response.g.dart';

/// JournalExportResultResponse
///
/// Properties:
/// * [batchRef] 
/// * [exportedCount] 
/// * [deadCount] 
/// * [lines] 
/// * [csvContent] 
@BuiltValue()
abstract class JournalExportResultResponse implements Built<JournalExportResultResponse, JournalExportResultResponseBuilder> {
  @BuiltValueField(wireName: r'batchRef')
  String? get batchRef;

  @BuiltValueField(wireName: r'exportedCount')
  int? get exportedCount;

  @BuiltValueField(wireName: r'deadCount')
  int? get deadCount;

  @BuiltValueField(wireName: r'lines')
  BuiltList<JournalExportLineResponse>? get lines;

  @BuiltValueField(wireName: r'csvContent')
  String? get csvContent;

  JournalExportResultResponse._();

  factory JournalExportResultResponse([void updates(JournalExportResultResponseBuilder b)]) = _$JournalExportResultResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(JournalExportResultResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<JournalExportResultResponse> get serializer => _$JournalExportResultResponseSerializer();
}

class _$JournalExportResultResponseSerializer implements PrimitiveSerializer<JournalExportResultResponse> {
  @override
  final Iterable<Type> types = const [JournalExportResultResponse, _$JournalExportResultResponse];

  @override
  final String wireName = r'JournalExportResultResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    JournalExportResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.batchRef != null) {
      yield r'batchRef';
      yield serializers.serialize(
        object.batchRef,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.exportedCount != null) {
      yield r'exportedCount';
      yield serializers.serialize(
        object.exportedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.deadCount != null) {
      yield r'deadCount';
      yield serializers.serialize(
        object.deadCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(JournalExportLineResponse)]),
      );
    }
    if (object.csvContent != null) {
      yield r'csvContent';
      yield serializers.serialize(
        object.csvContent,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    JournalExportResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required JournalExportResultResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'batchRef':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.batchRef = valueDes;
          break;
        case r'exportedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.exportedCount = valueDes;
          break;
        case r'deadCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.deadCount = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(JournalExportLineResponse)]),
          ) as BuiltList<JournalExportLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        case r'csvContent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.csvContent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  JournalExportResultResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = JournalExportResultResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}



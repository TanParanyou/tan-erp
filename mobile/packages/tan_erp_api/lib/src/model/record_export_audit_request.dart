//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'record_export_audit_request.g.dart';

/// RecordExportAuditRequest
///
/// Properties:
/// * [resourceType] 
/// * [rowCount] 
/// * [format] 
/// * [filename] 
/// * [filterSummary] 
@BuiltValue()
abstract class RecordExportAuditRequest implements Built<RecordExportAuditRequest, RecordExportAuditRequestBuilder> {
  @BuiltValueField(wireName: r'resourceType')
  String? get resourceType;

  @BuiltValueField(wireName: r'rowCount')
  int? get rowCount;

  @BuiltValueField(wireName: r'format')
  String? get format;

  @BuiltValueField(wireName: r'filename')
  String? get filename;

  @BuiltValueField(wireName: r'filterSummary')
  String? get filterSummary;

  RecordExportAuditRequest._();

  factory RecordExportAuditRequest([void updates(RecordExportAuditRequestBuilder b)]) = _$RecordExportAuditRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RecordExportAuditRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RecordExportAuditRequest> get serializer => _$RecordExportAuditRequestSerializer();
}

class _$RecordExportAuditRequestSerializer implements PrimitiveSerializer<RecordExportAuditRequest> {
  @override
  final Iterable<Type> types = const [RecordExportAuditRequest, _$RecordExportAuditRequest];

  @override
  final String wireName = r'RecordExportAuditRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RecordExportAuditRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.resourceType != null) {
      yield r'resourceType';
      yield serializers.serialize(
        object.resourceType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowCount != null) {
      yield r'rowCount';
      yield serializers.serialize(
        object.rowCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.format != null) {
      yield r'format';
      yield serializers.serialize(
        object.format,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.filename != null) {
      yield r'filename';
      yield serializers.serialize(
        object.filename,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.filterSummary != null) {
      yield r'filterSummary';
      yield serializers.serialize(
        object.filterSummary,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RecordExportAuditRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RecordExportAuditRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'resourceType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceType = valueDes;
          break;
        case r'rowCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.rowCount = valueDes;
          break;
        case r'format':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.format = valueDes;
          break;
        case r'filename':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.filename = valueDes;
          break;
        case r'filterSummary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.filterSummary = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RecordExportAuditRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RecordExportAuditRequestBuilder();
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



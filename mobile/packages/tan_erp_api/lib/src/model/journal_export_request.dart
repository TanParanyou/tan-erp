//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'journal_export_request.g.dart';

/// JournalExportRequest
///
/// Properties:
/// * [batchSize] 
@BuiltValue()
abstract class JournalExportRequest implements Built<JournalExportRequest, JournalExportRequestBuilder> {
  @BuiltValueField(wireName: r'batchSize')
  int? get batchSize;

  JournalExportRequest._();

  factory JournalExportRequest([void updates(JournalExportRequestBuilder b)]) = _$JournalExportRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(JournalExportRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<JournalExportRequest> get serializer => _$JournalExportRequestSerializer();
}

class _$JournalExportRequestSerializer implements PrimitiveSerializer<JournalExportRequest> {
  @override
  final Iterable<Type> types = const [JournalExportRequest, _$JournalExportRequest];

  @override
  final String wireName = r'JournalExportRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    JournalExportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.batchSize != null) {
      yield r'batchSize';
      yield serializers.serialize(
        object.batchSize,
        specifiedType: const FullType.nullable(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    JournalExportRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required JournalExportRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'batchSize':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.batchSize = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  JournalExportRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = JournalExportRequestBuilder();
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



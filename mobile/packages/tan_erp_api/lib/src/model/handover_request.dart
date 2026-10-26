//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'handover_request.g.dart';

/// HandoverRequest
///
/// Properties:
/// * [outcome] 
/// * [signerName] 
/// * [note] 
/// * [warrantyMonths] 
/// * [handoverDate] 
@BuiltValue()
abstract class HandoverRequest implements Built<HandoverRequest, HandoverRequestBuilder> {
  @BuiltValueField(wireName: r'outcome')
  String? get outcome;

  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'warrantyMonths')
  int? get warrantyMonths;

  @BuiltValueField(wireName: r'handoverDate')
  Date? get handoverDate;

  HandoverRequest._();

  factory HandoverRequest([void updates(HandoverRequestBuilder b)]) = _$HandoverRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(HandoverRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<HandoverRequest> get serializer => _$HandoverRequestSerializer();
}

class _$HandoverRequestSerializer implements PrimitiveSerializer<HandoverRequest> {
  @override
  final Iterable<Type> types = const [HandoverRequest, _$HandoverRequest];

  @override
  final String wireName = r'HandoverRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    HandoverRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.outcome != null) {
      yield r'outcome';
      yield serializers.serialize(
        object.outcome,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.signerName != null) {
      yield r'signerName';
      yield serializers.serialize(
        object.signerName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.warrantyMonths != null) {
      yield r'warrantyMonths';
      yield serializers.serialize(
        object.warrantyMonths,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.handoverDate != null) {
      yield r'handoverDate';
      yield serializers.serialize(
        object.handoverDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    HandoverRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required HandoverRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'outcome':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.outcome = valueDes;
          break;
        case r'signerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerName = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'warrantyMonths':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.warrantyMonths = valueDes;
          break;
        case r'handoverDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.handoverDate = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  HandoverRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = HandoverRequestBuilder();
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



//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_item_unit_conversion_request.g.dart';

/// CreateItemUnitConversionRequest
///
/// Properties:
/// * [fromUnitId] 
/// * [toUnitId] 
/// * [factor] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [reason] 
@BuiltValue()
abstract class CreateItemUnitConversionRequest implements Built<CreateItemUnitConversionRequest, CreateItemUnitConversionRequestBuilder> {
  @BuiltValueField(wireName: r'fromUnitId')
  String get fromUnitId;

  @BuiltValueField(wireName: r'toUnitId')
  String get toUnitId;

  @BuiltValueField(wireName: r'factor')
  String get factor;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'reason')
  String get reason;

  CreateItemUnitConversionRequest._();

  factory CreateItemUnitConversionRequest([void updates(CreateItemUnitConversionRequestBuilder b)]) = _$CreateItemUnitConversionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateItemUnitConversionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateItemUnitConversionRequest> get serializer => _$CreateItemUnitConversionRequestSerializer();
}

class _$CreateItemUnitConversionRequestSerializer implements PrimitiveSerializer<CreateItemUnitConversionRequest> {
  @override
  final Iterable<Type> types = const [CreateItemUnitConversionRequest, _$CreateItemUnitConversionRequest];

  @override
  final String wireName = r'CreateItemUnitConversionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateItemUnitConversionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'fromUnitId';
    yield serializers.serialize(
      object.fromUnitId,
      specifiedType: const FullType(String),
    );
    yield r'toUnitId';
    yield serializers.serialize(
      object.toUnitId,
      specifiedType: const FullType(String),
    );
    yield r'factor';
    yield serializers.serialize(
      object.factor,
      specifiedType: const FullType(String),
    );
    yield r'effectiveFrom';
    yield serializers.serialize(
      object.effectiveFrom,
      specifiedType: const FullType(Date),
    );
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType.nullable(Date),
      );
    }
    yield r'reason';
    yield serializers.serialize(
      object.reason,
      specifiedType: const FullType(String),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateItemUnitConversionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateItemUnitConversionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fromUnitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.fromUnitId = valueDes;
          break;
        case r'toUnitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.toUnitId = valueDes;
          break;
        case r'factor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.factor = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(Date),
          ) as Date;
          result.effectiveFrom = valueDes;
          break;
        case r'effectiveTo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveTo = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.reason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateItemUnitConversionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateItemUnitConversionRequestBuilder();
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



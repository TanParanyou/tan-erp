//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_unit_of_measure_request.g.dart';

/// UpdateUnitOfMeasureRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [symbol] 
/// * [dimension] 
/// * [decimalScale] 
/// * [roundingMode] 
@BuiltValue()
abstract class UpdateUnitOfMeasureRequest implements Built<UpdateUnitOfMeasureRequest, UpdateUnitOfMeasureRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextInput get name;

  @BuiltValueField(wireName: r'symbol')
  String get symbol;

  @BuiltValueField(wireName: r'dimension')
  String get dimension;

  @BuiltValueField(wireName: r'decimalScale')
  int? get decimalScale;

  @BuiltValueField(wireName: r'roundingMode')
  String? get roundingMode;

  UpdateUnitOfMeasureRequest._();

  factory UpdateUnitOfMeasureRequest([void updates(UpdateUnitOfMeasureRequestBuilder b)]) = _$UpdateUnitOfMeasureRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateUnitOfMeasureRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateUnitOfMeasureRequest> get serializer => _$UpdateUnitOfMeasureRequestSerializer();
}

class _$UpdateUnitOfMeasureRequestSerializer implements PrimitiveSerializer<UpdateUnitOfMeasureRequest> {
  @override
  final Iterable<Type> types = const [UpdateUnitOfMeasureRequest, _$UpdateUnitOfMeasureRequest];

  @override
  final String wireName = r'UpdateUnitOfMeasureRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateUnitOfMeasureRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'code';
    yield serializers.serialize(
      object.code,
      specifiedType: const FullType(String),
    );
    yield r'name';
    yield serializers.serialize(
      object.name,
      specifiedType: const FullType(LocalizedTextInput),
    );
    yield r'symbol';
    yield serializers.serialize(
      object.symbol,
      specifiedType: const FullType(String),
    );
    yield r'dimension';
    yield serializers.serialize(
      object.dimension,
      specifiedType: const FullType(String),
    );
    if (object.decimalScale != null) {
      yield r'decimalScale';
      yield serializers.serialize(
        object.decimalScale,
        specifiedType: const FullType(int),
      );
    }
    if (object.roundingMode != null) {
      yield r'roundingMode';
      yield serializers.serialize(
        object.roundingMode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateUnitOfMeasureRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateUnitOfMeasureRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(LocalizedTextInput),
          ) as LocalizedTextInput;
          result.name.replace(valueDes);
          break;
        case r'symbol':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.symbol = valueDes;
          break;
        case r'dimension':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.dimension = valueDes;
          break;
        case r'decimalScale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.decimalScale = valueDes;
          break;
        case r'roundingMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roundingMode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateUnitOfMeasureRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateUnitOfMeasureRequestBuilder();
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



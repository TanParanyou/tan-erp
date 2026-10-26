//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_unit_option_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_unit_options_response.g.dart';

/// EstimateUnitOptionsResponse
///
/// Properties:
/// * [conversionsEnabled] 
/// * [baseUnitCode] 
/// * [units] 
@BuiltValue()
abstract class EstimateUnitOptionsResponse implements Built<EstimateUnitOptionsResponse, EstimateUnitOptionsResponseBuilder> {
  @BuiltValueField(wireName: r'conversionsEnabled')
  bool? get conversionsEnabled;

  @BuiltValueField(wireName: r'baseUnitCode')
  String? get baseUnitCode;

  @BuiltValueField(wireName: r'units')
  BuiltList<EstimateUnitOptionResponse>? get units;

  EstimateUnitOptionsResponse._();

  factory EstimateUnitOptionsResponse([void updates(EstimateUnitOptionsResponseBuilder b)]) = _$EstimateUnitOptionsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateUnitOptionsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateUnitOptionsResponse> get serializer => _$EstimateUnitOptionsResponseSerializer();
}

class _$EstimateUnitOptionsResponseSerializer implements PrimitiveSerializer<EstimateUnitOptionsResponse> {
  @override
  final Iterable<Type> types = const [EstimateUnitOptionsResponse, _$EstimateUnitOptionsResponse];

  @override
  final String wireName = r'EstimateUnitOptionsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateUnitOptionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.conversionsEnabled != null) {
      yield r'conversionsEnabled';
      yield serializers.serialize(
        object.conversionsEnabled,
        specifiedType: const FullType(bool),
      );
    }
    if (object.baseUnitCode != null) {
      yield r'baseUnitCode';
      yield serializers.serialize(
        object.baseUnitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.units != null) {
      yield r'units';
      yield serializers.serialize(
        object.units,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateUnitOptionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateUnitOptionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateUnitOptionsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'conversionsEnabled':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.conversionsEnabled = valueDes;
          break;
        case r'baseUnitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.baseUnitCode = valueDes;
          break;
        case r'units':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateUnitOptionResponse)]),
          ) as BuiltList<EstimateUnitOptionResponse>?;
          if (valueDes == null) continue;
          result.units.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateUnitOptionsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateUnitOptionsResponseBuilder();
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



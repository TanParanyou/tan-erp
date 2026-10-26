//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculation_line_response.g.dart';

/// CalculationLineResponse
///
/// Properties:
/// * [lineId] 
/// * [workSubtype] 
/// * [billableQuantity] 
/// * [baseAmount] 
/// * [adjustedAmount] 
@BuiltValue()
abstract class CalculationLineResponse implements Built<CalculationLineResponse, CalculationLineResponseBuilder> {
  @BuiltValueField(wireName: r'lineId')
  String? get lineId;

  @BuiltValueField(wireName: r'workSubtype')
  String? get workSubtype;

  @BuiltValueField(wireName: r'billableQuantity')
  double? get billableQuantity;

  @BuiltValueField(wireName: r'baseAmount')
  double? get baseAmount;

  @BuiltValueField(wireName: r'adjustedAmount')
  double? get adjustedAmount;

  CalculationLineResponse._();

  factory CalculationLineResponse([void updates(CalculationLineResponseBuilder b)]) = _$CalculationLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculationLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculationLineResponse> get serializer => _$CalculationLineResponseSerializer();
}

class _$CalculationLineResponseSerializer implements PrimitiveSerializer<CalculationLineResponse> {
  @override
  final Iterable<Type> types = const [CalculationLineResponse, _$CalculationLineResponse];

  @override
  final String wireName = r'CalculationLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculationLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.lineId != null) {
      yield r'lineId';
      yield serializers.serialize(
        object.lineId,
        specifiedType: const FullType(String),
      );
    }
    if (object.workSubtype != null) {
      yield r'workSubtype';
      yield serializers.serialize(
        object.workSubtype,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.billableQuantity != null) {
      yield r'billableQuantity';
      yield serializers.serialize(
        object.billableQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.baseAmount != null) {
      yield r'baseAmount';
      yield serializers.serialize(
        object.baseAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.adjustedAmount != null) {
      yield r'adjustedAmount';
      yield serializers.serialize(
        object.adjustedAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculationLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculationLineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'lineId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lineId = valueDes;
          break;
        case r'workSubtype':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workSubtype = valueDes;
          break;
        case r'billableQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.billableQuantity = valueDes;
          break;
        case r'baseAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baseAmount = valueDes;
          break;
        case r'adjustedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.adjustedAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculationLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculationLineResponseBuilder();
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



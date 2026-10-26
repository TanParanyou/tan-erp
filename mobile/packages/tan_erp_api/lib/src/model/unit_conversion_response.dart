//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'unit_conversion_response.g.dart';

/// UnitConversionResponse
///
/// Properties:
/// * [id] 
/// * [fromUnitId] 
/// * [fromUnitCode] 
/// * [toUnitId] 
/// * [toUnitCode] 
/// * [factor] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [reason] 
/// * [status] 
/// * [rowVersion] 
@BuiltValue()
abstract class UnitConversionResponse implements Built<UnitConversionResponse, UnitConversionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'fromUnitId')
  String? get fromUnitId;

  @BuiltValueField(wireName: r'fromUnitCode')
  String? get fromUnitCode;

  @BuiltValueField(wireName: r'toUnitId')
  String? get toUnitId;

  @BuiltValueField(wireName: r'toUnitCode')
  String? get toUnitCode;

  @BuiltValueField(wireName: r'factor')
  String? get factor;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date? get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  UnitConversionResponse._();

  factory UnitConversionResponse([void updates(UnitConversionResponseBuilder b)]) = _$UnitConversionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UnitConversionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UnitConversionResponse> get serializer => _$UnitConversionResponseSerializer();
}

class _$UnitConversionResponseSerializer implements PrimitiveSerializer<UnitConversionResponse> {
  @override
  final Iterable<Type> types = const [UnitConversionResponse, _$UnitConversionResponse];

  @override
  final String wireName = r'UnitConversionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UnitConversionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.fromUnitId != null) {
      yield r'fromUnitId';
      yield serializers.serialize(
        object.fromUnitId,
        specifiedType: const FullType(String),
      );
    }
    if (object.fromUnitCode != null) {
      yield r'fromUnitCode';
      yield serializers.serialize(
        object.fromUnitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.toUnitId != null) {
      yield r'toUnitId';
      yield serializers.serialize(
        object.toUnitId,
        specifiedType: const FullType(String),
      );
    }
    if (object.toUnitCode != null) {
      yield r'toUnitCode';
      yield serializers.serialize(
        object.toUnitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.factor != null) {
      yield r'factor';
      yield serializers.serialize(
        object.factor,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.effectiveFrom != null) {
      yield r'effectiveFrom';
      yield serializers.serialize(
        object.effectiveFrom,
        specifiedType: const FullType(Date),
      );
    }
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UnitConversionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UnitConversionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'fromUnitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromUnitId = valueDes;
          break;
        case r'fromUnitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromUnitCode = valueDes;
          break;
        case r'toUnitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toUnitId = valueDes;
          break;
        case r'toUnitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toUnitCode = valueDes;
          break;
        case r'factor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.factor = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
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
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UnitConversionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UnitConversionResponseBuilder();
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



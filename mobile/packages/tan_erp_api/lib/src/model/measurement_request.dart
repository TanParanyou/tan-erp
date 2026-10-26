//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'measurement_request.g.dart';

/// MeasurementRequest
///
/// Properties:
/// * [lineId] 
/// * [workSubtype] 
/// * [widthM] 
/// * [heightM] 
/// * [depthM] 
/// * [quantity] 
@BuiltValue()
abstract class MeasurementRequest implements Built<MeasurementRequest, MeasurementRequestBuilder> {
  @BuiltValueField(wireName: r'lineId')
  String? get lineId;

  @BuiltValueField(wireName: r'workSubtype')
  String? get workSubtype;

  @BuiltValueField(wireName: r'widthM')
  double? get widthM;

  @BuiltValueField(wireName: r'heightM')
  double? get heightM;

  @BuiltValueField(wireName: r'depthM')
  double? get depthM;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  MeasurementRequest._();

  factory MeasurementRequest([void updates(MeasurementRequestBuilder b)]) = _$MeasurementRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MeasurementRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MeasurementRequest> get serializer => _$MeasurementRequestSerializer();
}

class _$MeasurementRequestSerializer implements PrimitiveSerializer<MeasurementRequest> {
  @override
  final Iterable<Type> types = const [MeasurementRequest, _$MeasurementRequest];

  @override
  final String wireName = r'MeasurementRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MeasurementRequest object, {
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
    if (object.widthM != null) {
      yield r'widthM';
      yield serializers.serialize(
        object.widthM,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.heightM != null) {
      yield r'heightM';
      yield serializers.serialize(
        object.heightM,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.depthM != null) {
      yield r'depthM';
      yield serializers.serialize(
        object.depthM,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MeasurementRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MeasurementRequestBuilder result,
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
        case r'widthM':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.widthM = valueDes;
          break;
        case r'heightM':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.heightM = valueDes;
          break;
        case r'depthM':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.depthM = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MeasurementRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MeasurementRequestBuilder();
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



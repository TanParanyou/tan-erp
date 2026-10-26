//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_conversion_snapshot_response.g.dart';

/// EstimateConversionSnapshotResponse
///
/// Properties:
/// * [conversionId] 
/// * [conversionScope] 
/// * [fromUnit] 
/// * [toUnit] 
/// * [factor] 
@BuiltValue()
abstract class EstimateConversionSnapshotResponse implements Built<EstimateConversionSnapshotResponse, EstimateConversionSnapshotResponseBuilder> {
  @BuiltValueField(wireName: r'conversionId')
  String? get conversionId;

  @BuiltValueField(wireName: r'conversionScope')
  String? get conversionScope;

  @BuiltValueField(wireName: r'fromUnit')
  String? get fromUnit;

  @BuiltValueField(wireName: r'toUnit')
  String? get toUnit;

  @BuiltValueField(wireName: r'factor')
  String? get factor;

  EstimateConversionSnapshotResponse._();

  factory EstimateConversionSnapshotResponse([void updates(EstimateConversionSnapshotResponseBuilder b)]) = _$EstimateConversionSnapshotResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateConversionSnapshotResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateConversionSnapshotResponse> get serializer => _$EstimateConversionSnapshotResponseSerializer();
}

class _$EstimateConversionSnapshotResponseSerializer implements PrimitiveSerializer<EstimateConversionSnapshotResponse> {
  @override
  final Iterable<Type> types = const [EstimateConversionSnapshotResponse, _$EstimateConversionSnapshotResponse];

  @override
  final String wireName = r'EstimateConversionSnapshotResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateConversionSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.conversionId != null) {
      yield r'conversionId';
      yield serializers.serialize(
        object.conversionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.conversionScope != null) {
      yield r'conversionScope';
      yield serializers.serialize(
        object.conversionScope,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fromUnit != null) {
      yield r'fromUnit';
      yield serializers.serialize(
        object.fromUnit,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.toUnit != null) {
      yield r'toUnit';
      yield serializers.serialize(
        object.toUnit,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateConversionSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateConversionSnapshotResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'conversionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.conversionId = valueDes;
          break;
        case r'conversionScope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.conversionScope = valueDes;
          break;
        case r'fromUnit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromUnit = valueDes;
          break;
        case r'toUnit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toUnit = valueDes;
          break;
        case r'factor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.factor = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateConversionSnapshotResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateConversionSnapshotResponseBuilder();
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



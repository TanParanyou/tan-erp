//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculation_snapshot_response.g.dart';

/// CalculationSnapshotResponse
///
/// Properties:
/// * [version] 
/// * [snapshotJson] 
@BuiltValue()
abstract class CalculationSnapshotResponse implements Built<CalculationSnapshotResponse, CalculationSnapshotResponseBuilder> {
  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'snapshotJson')
  String? get snapshotJson;

  CalculationSnapshotResponse._();

  factory CalculationSnapshotResponse([void updates(CalculationSnapshotResponseBuilder b)]) = _$CalculationSnapshotResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculationSnapshotResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculationSnapshotResponse> get serializer => _$CalculationSnapshotResponseSerializer();
}

class _$CalculationSnapshotResponseSerializer implements PrimitiveSerializer<CalculationSnapshotResponse> {
  @override
  final Iterable<Type> types = const [CalculationSnapshotResponse, _$CalculationSnapshotResponse];

  @override
  final String wireName = r'CalculationSnapshotResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculationSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.snapshotJson != null) {
      yield r'snapshotJson';
      yield serializers.serialize(
        object.snapshotJson,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculationSnapshotResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculationSnapshotResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'snapshotJson':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.snapshotJson = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculationSnapshotResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculationSnapshotResponseBuilder();
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



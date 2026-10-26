//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_snapshot_summary_response.g.dart';

/// MrpSnapshotSummaryResponse
///
/// Properties:
/// * [demandCount] 
/// * [supplyCount] 
/// * [bomCount] 
/// * [stockItemCount] 
@BuiltValue()
abstract class MrpSnapshotSummaryResponse implements Built<MrpSnapshotSummaryResponse, MrpSnapshotSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'demandCount')
  int? get demandCount;

  @BuiltValueField(wireName: r'supplyCount')
  int? get supplyCount;

  @BuiltValueField(wireName: r'bomCount')
  int? get bomCount;

  @BuiltValueField(wireName: r'stockItemCount')
  int? get stockItemCount;

  MrpSnapshotSummaryResponse._();

  factory MrpSnapshotSummaryResponse([void updates(MrpSnapshotSummaryResponseBuilder b)]) = _$MrpSnapshotSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpSnapshotSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpSnapshotSummaryResponse> get serializer => _$MrpSnapshotSummaryResponseSerializer();
}

class _$MrpSnapshotSummaryResponseSerializer implements PrimitiveSerializer<MrpSnapshotSummaryResponse> {
  @override
  final Iterable<Type> types = const [MrpSnapshotSummaryResponse, _$MrpSnapshotSummaryResponse];

  @override
  final String wireName = r'MrpSnapshotSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpSnapshotSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.demandCount != null) {
      yield r'demandCount';
      yield serializers.serialize(
        object.demandCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.supplyCount != null) {
      yield r'supplyCount';
      yield serializers.serialize(
        object.supplyCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.bomCount != null) {
      yield r'bomCount';
      yield serializers.serialize(
        object.bomCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.stockItemCount != null) {
      yield r'stockItemCount';
      yield serializers.serialize(
        object.stockItemCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpSnapshotSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpSnapshotSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'demandCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.demandCount = valueDes;
          break;
        case r'supplyCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.supplyCount = valueDes;
          break;
        case r'bomCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.bomCount = valueDes;
          break;
        case r'stockItemCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.stockItemCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpSnapshotSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpSnapshotSummaryResponseBuilder();
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



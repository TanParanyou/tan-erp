//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/mrp_recommendation_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/mrp_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/mrp_snapshot_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_run_response.g.dart';

/// MrpRunResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [asOfDate] 
/// * [purchaseLeadTimeDays] 
/// * [productionLeadTimeDays] 
/// * [inputHash] 
/// * [snapshot] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [recommendations] 
@BuiltValue()
abstract class MrpRunResponse implements Built<MrpRunResponse, MrpRunResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'asOfDate')
  Date? get asOfDate;

  @BuiltValueField(wireName: r'purchaseLeadTimeDays')
  int? get purchaseLeadTimeDays;

  @BuiltValueField(wireName: r'productionLeadTimeDays')
  int? get productionLeadTimeDays;

  @BuiltValueField(wireName: r'inputHash')
  String? get inputHash;

  @BuiltValueField(wireName: r'snapshot')
  MrpSnapshotSummaryResponse? get snapshot;

  @BuiltValueField(wireName: r'createdBy')
  MrpPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'recommendations')
  BuiltList<MrpRecommendationResponse>? get recommendations;

  MrpRunResponse._();

  factory MrpRunResponse([void updates(MrpRunResponseBuilder b)]) = _$MrpRunResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpRunResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpRunResponse> get serializer => _$MrpRunResponseSerializer();
}

class _$MrpRunResponseSerializer implements PrimitiveSerializer<MrpRunResponse> {
  @override
  final Iterable<Type> types = const [MrpRunResponse, _$MrpRunResponse];

  @override
  final String wireName = r'MrpRunResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpRunResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.asOfDate != null) {
      yield r'asOfDate';
      yield serializers.serialize(
        object.asOfDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.purchaseLeadTimeDays != null) {
      yield r'purchaseLeadTimeDays';
      yield serializers.serialize(
        object.purchaseLeadTimeDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.productionLeadTimeDays != null) {
      yield r'productionLeadTimeDays';
      yield serializers.serialize(
        object.productionLeadTimeDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.inputHash != null) {
      yield r'inputHash';
      yield serializers.serialize(
        object.inputHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.snapshot != null) {
      yield r'snapshot';
      yield serializers.serialize(
        object.snapshot,
        specifiedType: const FullType(MrpSnapshotSummaryResponse),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(MrpPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.recommendations != null) {
      yield r'recommendations';
      yield serializers.serialize(
        object.recommendations,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MrpRecommendationResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpRunResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpRunResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'asOfDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.asOfDate = valueDes;
          break;
        case r'purchaseLeadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.purchaseLeadTimeDays = valueDes;
          break;
        case r'productionLeadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.productionLeadTimeDays = valueDes;
          break;
        case r'inputHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inputHash = valueDes;
          break;
        case r'snapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpSnapshotSummaryResponse),
          ) as MrpSnapshotSummaryResponse?;
          if (valueDes == null) continue;
          result.snapshot.replace(valueDes);
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpPersonResponse),
          ) as MrpPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'recommendations':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MrpRecommendationResponse)]),
          ) as BuiltList<MrpRecommendationResponse>?;
          if (valueDes == null) continue;
          result.recommendations.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpRunResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpRunResponseBuilder();
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



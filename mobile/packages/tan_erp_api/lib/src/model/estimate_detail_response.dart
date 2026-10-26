//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/estimate_revision_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_detail_response.g.dart';

/// EstimateDetailResponse
///
/// Properties:
/// * [id] 
/// * [organizationId] 
/// * [branchId] 
/// * [customerId] 
/// * [opportunityId] 
/// * [siteSurveyRevisionId] 
/// * [siteSurveySnapshotHash] 
/// * [number] 
/// * [status] 
/// * [currentRevisionNo] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [updatedAtUtc] 
/// * [currentRevision] 
@BuiltValue()
abstract class EstimateDetailResponse implements Built<EstimateDetailResponse, EstimateDetailResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organizationId')
  String? get organizationId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'siteSurveyRevisionId')
  String? get siteSurveyRevisionId;

  @BuiltValueField(wireName: r'siteSurveySnapshotHash')
  String? get siteSurveySnapshotHash;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'currentRevisionNo')
  int? get currentRevisionNo;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'currentRevision')
  EstimateRevisionResponse? get currentRevision;

  EstimateDetailResponse._();

  factory EstimateDetailResponse([void updates(EstimateDetailResponseBuilder b)]) = _$EstimateDetailResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateDetailResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateDetailResponse> get serializer => _$EstimateDetailResponseSerializer();
}

class _$EstimateDetailResponseSerializer implements PrimitiveSerializer<EstimateDetailResponse> {
  @override
  final Iterable<Type> types = const [EstimateDetailResponse, _$EstimateDetailResponse];

  @override
  final String wireName = r'EstimateDetailResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateDetailResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.organizationId != null) {
      yield r'organizationId';
      yield serializers.serialize(
        object.organizationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.siteSurveyRevisionId != null) {
      yield r'siteSurveyRevisionId';
      yield serializers.serialize(
        object.siteSurveyRevisionId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.siteSurveySnapshotHash != null) {
      yield r'siteSurveySnapshotHash';
      yield serializers.serialize(
        object.siteSurveySnapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
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
    if (object.currentRevisionNo != null) {
      yield r'currentRevisionNo';
      yield serializers.serialize(
        object.currentRevisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.currentRevision != null) {
      yield r'currentRevision';
      yield serializers.serialize(
        object.currentRevision,
        specifiedType: const FullType(EstimateRevisionResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateDetailResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateDetailResponseBuilder result,
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
        case r'organizationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.organizationId = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'siteSurveyRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyRevisionId = valueDes;
          break;
        case r'siteSurveySnapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveySnapshotHash = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'currentRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.currentRevisionNo = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'currentRevision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateRevisionResponse),
          ) as EstimateRevisionResponse?;
          if (valueDes == null) continue;
          result.currentRevision.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateDetailResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateDetailResponseBuilder();
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



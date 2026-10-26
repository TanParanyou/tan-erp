//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_site_response.dart';
import 'package:tan_erp_api/src/model/project_customer_response.dart';
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/project_opportunity_response.dart';
import 'package:tan_erp_api/src/model/project_baseline_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_response.g.dart';

/// ProjectResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [code] 
/// * [name] 
/// * [status] 
/// * [plannedStartDate] 
/// * [owner] 
/// * [customer] 
/// * [site] 
/// * [opportunity] 
/// * [baseline] 
/// * [rowVersion] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class ProjectResponse implements Built<ProjectResponse, ProjectResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'plannedStartDate')
  Date? get plannedStartDate;

  @BuiltValueField(wireName: r'owner')
  ProjectPersonResponse? get owner;

  @BuiltValueField(wireName: r'customer')
  ProjectCustomerResponse? get customer;

  @BuiltValueField(wireName: r'site')
  ProjectSiteResponse? get site;

  @BuiltValueField(wireName: r'opportunity')
  ProjectOpportunityResponse? get opportunity;

  @BuiltValueField(wireName: r'baseline')
  ProjectBaselineResponse? get baseline;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  ProjectResponse._();

  factory ProjectResponse([void updates(ProjectResponseBuilder b)]) = _$ProjectResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectResponse> get serializer => _$ProjectResponseSerializer();
}

class _$ProjectResponseSerializer implements PrimitiveSerializer<ProjectResponse> {
  @override
  final Iterable<Type> types = const [ProjectResponse, _$ProjectResponse];

  @override
  final String wireName = r'ProjectResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
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
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
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
    if (object.plannedStartDate != null) {
      yield r'plannedStartDate';
      yield serializers.serialize(
        object.plannedStartDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.owner != null) {
      yield r'owner';
      yield serializers.serialize(
        object.owner,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.customer != null) {
      yield r'customer';
      yield serializers.serialize(
        object.customer,
        specifiedType: const FullType(ProjectCustomerResponse),
      );
    }
    if (object.site != null) {
      yield r'site';
      yield serializers.serialize(
        object.site,
        specifiedType: const FullType(ProjectSiteResponse),
      );
    }
    if (object.opportunity != null) {
      yield r'opportunity';
      yield serializers.serialize(
        object.opportunity,
        specifiedType: const FullType(ProjectOpportunityResponse),
      );
    }
    if (object.baseline != null) {
      yield r'baseline';
      yield serializers.serialize(
        object.baseline,
        specifiedType: const FullType(ProjectBaselineResponse),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'plannedStartDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedStartDate = valueDes;
          break;
        case r'owner':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.owner.replace(valueDes);
          break;
        case r'customer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectCustomerResponse),
          ) as ProjectCustomerResponse?;
          if (valueDes == null) continue;
          result.customer.replace(valueDes);
          break;
        case r'site':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectSiteResponse),
          ) as ProjectSiteResponse?;
          if (valueDes == null) continue;
          result.site.replace(valueDes);
          break;
        case r'opportunity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectOpportunityResponse),
          ) as ProjectOpportunityResponse?;
          if (valueDes == null) continue;
          result.opportunity.replace(valueDes);
          break;
        case r'baseline':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectBaselineResponse),
          ) as ProjectBaselineResponse?;
          if (valueDes == null) continue;
          result.baseline.replace(valueDes);
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectResponseBuilder();
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


